using System;
using AppTasks.Models;
using AppTasks.Services;
using Plugin.Media;
using Xamarin.Forms;

namespace AppTasks.ViewModels
{
    public class TasksDetailViewModel : BaseViewModel
    {
        Command takePictureCommand;
        public Command TakePictureCommand => takePictureCommand ?? (takePictureCommand = new Command(TakePictureAction));

        Command selectPictureCommand;
        public Command SelectPictureCommand => selectPictureCommand ?? (selectPictureCommand = new Command(SelectPictureAction));

        Command saveCommand;
        public Command SaveCommand => saveCommand ?? (saveCommand = new Command(SaveAction));

        Command deleteCommand;
        public Command DeleteCommand => deleteCommand ?? (deleteCommand = new Command(DeleteAction));

        Command cancelCommand;
        public Command CancelCommand => cancelCommand ?? (cancelCommand = new Command(CancelAction));

        TaskModel taskSelected;
        public TaskModel TaskSelected
        {
            get => taskSelected;
            set => SetProperty(ref taskSelected, value);
        }

        string imageBase64;
        public string ImageBase64
        {
            get => imageBase64;
            set => SetProperty(ref imageBase64, value);
        }

        /*ImageSource imageSource_;
        public ImageSource ImageSource_
        {
            get => imageSource_;
            set => SetProperty(ref imageSource_, value);
        }*/

        public TasksDetailViewModel()
        {
            TaskSelected = new TaskModel();
        }

        public TasksDetailViewModel(TaskModel taskSelected)
        {
            // De la imagen guardada en SQLite en formato base 64, hacemos la conversión para visualizarla en imagen
            /*if(!string.IsNullOrEmpty(taskSelected.ImageBase64))
            {
                ImageSource_ = new ImageService().ConvertImageFromBase64ToImageSource(taskSelected.ImageBase64);
            }*/

            TaskSelected = taskSelected;
            ImageBase64 = taskSelected.ImageBase64;
        }

        private async void TakePictureAction()
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                await CrossMedia.Current.Initialize();
            }

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg"
            });

            if (file == null)
                return;

            TaskSelected.ImageBase64 = ImageBase64 = await new ImageService().ConvertImageFileToBase64(file.Path);
        }

        private async void SelectPictureAction()
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                await CrossMedia.Current.Initialize();
            }

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Seleccionar fotografías no soportada", "OK");
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });

            if (file == null)
                return;

            TaskSelected.ImageBase64 = ImageBase64 = await new ImageService().ConvertImageFileToBase64(file.Path);
        }

        private async void SaveAction()
        {
            // Descargamos la imagen con la URL tecleada y la convertimos a base 64 para que se guarde en SQLite
            /*if (!string.IsNullOrEmpty(TaskSelected.ImageUrl))
            {
                TaskSelected.ImageBase64 = await new ImageService().DownloadImageAsBase64Async(TaskSelected.ImageUrl);
            }*/

            // Guardamos la tarea en SQLite
            await App.TasksDatabase.SaveTaskAsync(TaskSelected);
            // Refescamos las tareas en el listado
            TasksListViewModel.GetInstance().LoadTasks();
            // Cerramos la ventana
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void DeleteAction()
        {
            // Borramos la tarea en SQLite
            await App.TasksDatabase.DeleteTaskAsync(TaskSelected);
            // Refescamos las tareas en el listado
            TasksListViewModel.GetInstance().LoadTasks();
            // Cerramos la ventana
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void CancelAction()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
