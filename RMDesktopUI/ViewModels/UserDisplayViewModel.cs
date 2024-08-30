using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Models;
using RMDesktopUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RMDesktopUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private readonly IUserEndpoint _userEndpoint;

        private BindingList<UserModel> _users;

        public BindingList<UserModel> Users
        {
            get 
            {
                return _users; 
            }
            set 
            {
                _users = value;
                NotifyOfPropertyChange(() => Users);
            }
        }


        public UserDisplayViewModel(StatusInfoViewModel status, IWindowManager window, IUserEndpoint userEndpoint )
        {
            _status = status;
            _window = window;
            _userEndpoint = userEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadUsers();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";
                var info = IoC.Get<StatusInfoViewModel>();

                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unathorized Access", "You do not have permission to interact with the Sales Form.");
                    _window.ShowDialog(_status, null, settings);
                }
                else
                {
                    _status.UpdateMessage("Fatal Exception", ex.Message);
                    _window.ShowDialog(_status, null, settings);
                }

                TryClose();
            }
        }

        private async Task LoadUsers()
        {
            var usersList = await _userEndpoint.GetAll();
            Users = new BindingList<UserModel>(usersList);
        }

    }
}
