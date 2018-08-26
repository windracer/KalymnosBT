using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KalymnosBT.MVVMBase
{
    public interface IUndoCommand : ICommand
    {
        void Undo();
    }

    public class UndoManager
    {
        private readonly List<IUndoCommand> _undoList = new List<IUndoCommand>();
        private readonly List<IUndoCommand> _redoList = new List<IUndoCommand>();
        public int UndoLimit { get; private set; }

        public UndoManager(int limit = 0) => UndoLimit = limit;

        public virtual bool CanUndo => _undoList.Count > 0;
        public virtual bool CanRedo => _redoList.Count > 0;

        public virtual void Undo()
        {
            if (!CanUndo)
            {
                return;
            }

            var index = _undoList.Count - 1;
            _undoList[index].Undo();
            _redoList.Add(_undoList[index]);
            _undoList.RemoveAt(index);
        }

        public virtual void Redo()
        {
            if (!CanRedo)
                return;

            var index = _redoList.Count - 1;
            var cmd = _redoList[index];
            cmd.Execute(null);
            _redoList.RemoveAt(index);
            _undoList.Add(cmd);
        }

        public void AddCommand(IUndoCommand cmd)
        {
            _undoList.Add(cmd);
            _redoList.Clear();
            if (UndoLimit > 0 && _undoList.Count > UndoLimit)
                _undoList.RemoveAt(0);
        }

    }

    public class ReversableCommand : ICommand
    {
        private readonly IUndoCommand _command;
        private readonly UndoManager _undoManager;

        public ReversableCommand(UndoManager mngr, IUndoCommand cmd)
        {
            _undoManager = mngr;
            _command = cmd;
        }


        public bool CanExecute(object parameter)
        {
            return _command.CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _command.Execute(parameter);
            _undoManager.AddCommand(_command);
        }

        public event EventHandler CanExecuteChanged
        {
            add => _command.CanExecuteChanged += value; 
            remove => _command.CanExecuteChanged -= value; 
        }
    }
}
