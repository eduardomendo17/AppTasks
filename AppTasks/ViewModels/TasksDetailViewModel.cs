using System;
using AppTasks.Models;
using Xamarin.Forms;

namespace AppTasks.ViewModels
{
    public class TasksDetailViewModel : BaseViewModel
    {
        Command saveCommand;
        public Command SaveCommand => saveCommand ?? (saveCommand = new Command(SaveAction));

        Command deleteCommand;
        public Command DeleteCommand => deleteCommand ?? (deleteCommand = new Command(DeleteAction));

        Command cancelCommand;
        public Command CancelCommand => cancelCommand ?? (cancelCommand = new Command(CancelAction));

        /*string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        string notes;
        public string Notes
        {
            get => notes;
            set => SetProperty(ref notes, value);
        }

        bool done;
        public bool Done
        {
            get => done;
            set => SetProperty(ref done, value);
        }*/

        TaskModel taskSelected;
        public TaskModel TaskSelected
        {
            get => taskSelected;
            set => SetProperty(ref taskSelected, value);
        }

        public TasksDetailViewModel()
        {
            TaskSelected = new TaskModel();
        }

        public TasksDetailViewModel(TaskModel taskSelected)
        {
            TaskSelected = taskSelected;

            /*Name = taskSelected.Name;
            Notes = taskSelected.Notes;
            Done = taskSelected.Done;*/
        }

        private async void SaveAction()
        {
            await App.TasksDatabase.SaveTaskAsync(TaskSelected);
            TasksListViewModel.GetInstance().LoadTasks();
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void DeleteAction()
        {
            await App.TasksDatabase.DeleteTaskAsync(TaskSelected);
            TasksListViewModel.GetInstance().LoadTasks();
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void CancelAction()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
