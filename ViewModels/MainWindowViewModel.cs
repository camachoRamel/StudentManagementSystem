﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StudentManagementSystem.Helper;
using StudentManagementSystem.Messages;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {

        private readonly IMessenger _messenger = WeakReferenceMessenger.Default;
        public MainWindowViewModel()
        {
            _messenger.Register<MainWindowViewModel, LoginSuccessMessage>(this, (_, messaage) =>
            {
                Auth.CurrentUser = new User
                {
                    Id = messaage.Value.Id

                };

                CurrentPage = new HomePageViewModel();
            });
        }

        [ObservableProperty]
        private bool _isPaneOpen = true;

        [ObservableProperty]
        private ViewModelBase _currentPage = new LoginSignupPageViewModel();

        public ObservableCollection<ListItemTemplate> Items { get; } = new ObservableCollection<ListItemTemplate>()
        {
            new ListItemTemplate(typeof(HomePageViewModel), "home_regular"),
            new ListItemTemplate(typeof(ClassPageViewModel), "guest_regular"),
        };
        
        // Footer navigation items
        public ObservableCollection<ListItemTemplate> FooterItems { get; } = new ObservableCollection<ListItemTemplate>()
        {
            new ListItemTemplate(typeof(SettingsPageViewModel), "people_settings_regular"),
            new ListItemTemplate(typeof(LoginSignupPageViewModel), "person_regular")
        };

        [ObservableProperty] 
        private ListItemTemplate? _selectedFooterItem;

        [ObservableProperty] private ListItemTemplate? _selectedListItem;

        partial void OnSelectedListItemChanged(ListItemTemplate? value)
        {
            if (value is null) return;
            var instance = Activator.CreateInstance(value.ModelType);
            if(instance is null) return;
            CurrentPage = (ViewModelBase)instance;
        }
        
        partial void OnSelectedFooterItemChanged(ListItemTemplate? value)
        {
            if (value is null) return;
            var instance = Activator.CreateInstance(value.ModelType);
            if (instance is null) return;
            CurrentPage = (ViewModelBase)instance;
        }

        [RelayCommand]
        private void TriggerPane() 
        {
            IsPaneOpen = !IsPaneOpen;
        }
    }

    public class ListItemTemplate
    {
        public ListItemTemplate(Type type, string iconKey)
        {
            ModelType = type;
            Label = type.Name.Replace("PageViewModel", "");

            Application.Current!.TryFindResource(iconKey, out var res);
            ListItemIcon = (StreamGeometry)res;
        }

        public string Label { get; set; }
        public Type ModelType { get; }
        public StreamGeometry ListItemIcon { get; }
    }

}
