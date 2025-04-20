using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace SmsClientLibrary.WpfClient.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<EnvVarModel> Variables { get; set; } = new();

    public ICommand SaveCommand { get; }

    public MainViewModel()
    {
        LoadEnvironmentVariables();

        SaveCommand = new RelayCommand(async () => await SaveAllAsync());
    }

    private void LoadEnvironmentVariables()
    {
        var names =
            App.Configuration.GetSection("EnvironmentVariables").Get<string[]>()
            ?? Array.Empty<string>();
        foreach (var name in names)
        {
            var value =
                Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User)
                ?? string.Empty;
            Variables.Add(new EnvVarModel { Name = name, Value = value });
        }
    }

    private async Task SaveAllAsync()
    {
        IsBusy = true;

        try
        {
            await Task.Run(() =>
            {
                foreach (var variable in Variables)
                {
                    var oldValue =
                        Environment.GetEnvironmentVariable(
                            variable.Name,
                            EnvironmentVariableTarget.User
                        ) ?? "(null)";

                    Environment.SetEnvironmentVariable(
                        variable.Name,
                        variable.Value,
                        EnvironmentVariableTarget.User
                    );

                    Log.Information(
                        "Changed {Name} from '{Old}' to '{New}'",
                        variable.Name,
                        oldValue,
                        variable.Value
                    );
                }
            });

            MessageBox.Show(
                "Environment variables saved successfully.",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
        finally
        {
            IsBusy = false;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}

/// <summary>Не использовал здесь CommunityToolKit поэтому реализовал затычку.</summary>
public class RelayCommand : ICommand
{
    private readonly Action _execute;

    public RelayCommand(Action execute) => _execute = execute;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter) => _execute();

    public event EventHandler CanExecuteChanged;
}
