﻿using Caliburn.Micro;
using RMDesktopUI.EventModels;
using RMDesktopUI.Helpers;
using RMDesktopUI.Library.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
		private string _userName = "jeancarlosrt301@gmail.com";
		private string _password = "Pwd12345.";
		private IAPIHelper _apiHelper;
		private IEventAggregator _events;
        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events)
        {
            _apiHelper = apiHelper;
			_events = events;
        }

        public string UserName
		{
			get { return _userName; }
			set 
			{
				_userName = value;
				NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }


		public string Password
		{
			get { return _password; }
			set 
			{
				_password = value;
				NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

		public bool CanLogIn
		{
			get
			{
                bool output = false;

                if (UserName?.Length > 0 && Password?.Length > 0)
                {
                    output = true;
                }

                return output;
            }
		}

		public bool IsErrorVisible
        {
			get 
			{
				bool output = false;

				if (ErrorMessage?.Length > 0)
				{
					output = true;
				}

				return output; 
			}
		}

		private string _errorMessage;

		public string ErrorMessage
		{
			get { return _errorMessage; }
			set 
			{
				_errorMessage = value; 
                NotifyOfPropertyChange(() => ErrorMessage);
                NotifyOfPropertyChange(() => IsErrorVisible);
			}
		}


		public async Task LogIn()
		{
			try
			{
				ErrorMessage = "";
				var result = await _apiHelper.Authenticate(UserName, Password);

				// Capture more information about the user
				await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

				await _events.PublishOnUIThreadAsync(new LogOnEvent());
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
        }

	}
}
