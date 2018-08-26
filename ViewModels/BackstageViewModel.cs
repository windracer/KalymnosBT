using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using KalymnosBT.Models;
using KalymnosBT.MVVMBase;
using KalymnosBT.Views;

namespace KalymnosBT.ViewModels
{
    class BackstageViewModel : ViewModelBase<KalymnosDb>
    {
        public Window TheView { get; set; }
        private Project _selectedProject;
        public Project SelectedProject { get => _selectedProject; set => SetProperty(ref _selectedProject, value); }

        private ICommand _addProjectCommand;
        private ICommand _editProjectCommand;

        public ICommand AddProjectCommand
        {
            get
            {
                return _addProjectCommand ?? (_addProjectCommand = new RelayCommand(
                           () =>
                           {
                               var project = new Project();
                               var dlg = new ProjectDetailsWindow{Owner = TheView, DataContext = project, ShowDelButton = false};

                               if (dlg.ShowDialog() == true)
                               {
                                   Model.Projects.Add(project);
                                   SelectedProject = project;

                                   KalymnosDbHelper.Save(Model);
                               }
                           }));
            }
        }

        public ICommand EditProjectCommand
        {
            get
            {
                return _editProjectCommand ?? (_editProjectCommand = new RelayCommand(
                           () =>
                           {
                               var project = new Project
                               {
                                   Name = SelectedProject.Name,
                                   Prefix = SelectedProject.Prefix,
                                   LastIssueNumber = SelectedProject.LastIssueNumber
                               };
                               var dlg = new ProjectDetailsWindow {Owner = TheView, DataContext = project, ShowDelButton = true};

                               if (dlg.ShowDialog() == true)
                               {
                                   SelectedProject.Name = project.Name;
                                   SelectedProject.Prefix = project.Prefix;
                                   SelectedProject.LastIssueNumber = project.LastIssueNumber;

                                   if (dlg.WasDelButtonPressed)
                                   {
                                       Model.Trash.Add(new Tuple<Project, DateTime>(SelectedProject, DateTime.UtcNow));
                                       Model.Projects.Remove(SelectedProject);
                                   }
                                   KalymnosDbHelper.Save(Model);
                               }
                           }));
            }
        }

    }
}
