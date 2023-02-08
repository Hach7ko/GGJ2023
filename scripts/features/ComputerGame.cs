using Godot;
using System;
using System.Collections.Generic;

public class ComputerGame : ObjectOfInterestFeature
{
    private class FileSystem
    {
        public String                       Name = "unknown";
        public FileSystem                   Parent = null;
        public List<FileSystem>             Children = new List<FileSystem>();
        public List<Tuple<String, String, Boolean>>  Files = new List<Tuple<String, String, Boolean>>();

        public FileSystem(String name)
        {
            Name = name;
        }

        public void AddChildren(FileSystem fs)
        {
            fs.Parent = this;
            Children.Add(fs);
        }
    }

    private FileSystem  _fs;
    private FileSystem  _currentFolder;
    private Control     _fileArea;

//-----------------------------------------------------------------------------
    public override void _Ready()
    {
        Hide();
        GetNode<Control>("Notepad").Hide();
        GetNode<Control>("PasswordProtected").Hide();

        _fileArea = GetNode<Control>("FileArea");

        FileSystem dir0 = new FileSystem("Documents");
            dir0.Files.Add(new Tuple<String, String, Boolean>("New File.txt", "Hello, world!", false));
            dir0.Files.Add(new Tuple<String, String, Boolean>("Credits.txt", "Sumokuchan and Roulyo, with <3", false));
            FileSystem dir00 = new FileSystem("Secret!");
                dir00.Files.Add(new Tuple<String, String, Boolean>("Diary.txt", "Dear diary...", true));
        FileSystem dir1 = new FileSystem("Programs");

        _fs = new FileSystem("C:");
        _fs.Files.Add(new Tuple<String, String, Boolean>("clue.txt", "8", false));
        _fs.AddChildren(dir0);
            dir0.AddChildren(dir00);
        _fs.AddChildren(dir1);

        _currentFolder = dir0;

        UpdateScreen();
    }

//-----------------------------------------------------------------------------
    private void UpdateScreen()
    {
        ClearFolderArea();
        UpdateFolderArea();
        UpdatePath();
    }

//-----------------------------------------------------------------------------
    private void ClearFolderArea()
    {
        foreach (Node child in _fileArea.GetChildren())
        {
            _fileArea.RemoveChild(child);
            child.QueueFree();
        }
    }

//-----------------------------------------------------------------------------
    private void UpdateFolderArea()
    {
        int fileCount = 0;
        foreach (FileSystem file in _currentFolder.Children)
        {
            Icon icon = CreateIcon(fileCount);
            icon.GetNode<Label>("Label").Text = file.Name;
            icon.GetNode<Button>("FileIcon").Hide();
            icon.Connect(nameof(Icon.DoubleClick), this, nameof(OnFolderDoubleClick));

            ++fileCount;
        }

        foreach (Tuple<String, String, Boolean> file in _currentFolder.Files)
        {
            Icon icon = CreateIcon(fileCount);
            icon.GetNode<Label>("Label").Text = file.Item1;
            icon.GetNode<Button>("FolderIcon").Hide();
            icon.Connect(nameof(Icon.DoubleClick), this, nameof(OnFileDoubleClick));

            ++fileCount;
        }
    }

//-----------------------------------------------------------------------------
    private void UpdatePath()
    {
        String path = "";
        FileSystem folder = _currentFolder;

        while (folder.Parent != null)
        {
            path = folder.Name + "/" + path;
            folder = folder.Parent;
        }

        path = "C:/" + path;

        GetNode<Label>("Control/Path").Text = path;
    }

//-----------------------------------------------------------------------------
    private Icon CreateIcon(int index)
    {
        Icon icon = (Icon) ((PackedScene)ResourceLoader.Load("res://scenes/hud/Icon.tscn")).Instance();
        icon.Index = index;
        icon.RectPosition = new Vector2(
            x: (index % 8) * icon.RectSize.x,
            y: (index / 8) * icon.RectSize.y
        );
        _fileArea.AddChild(icon);

        return icon;
    }

//-----------------------------------------------------------------------------
    private void OnFolderDoubleClick(int index)
    {
        _currentFolder = _currentFolder.Children[index];

        UpdateScreen();
    }

//-----------------------------------------------------------------------------
    private void OnFileDoubleClick(int index)
    {
        Control notepad = GetNode<Control>("Notepad");
        int fileIndex = index - _currentFolder.Children.Count;
        bool isPasswordProtected = _currentFolder.Files[fileIndex].Item3;
        notepad.GetNode<TextEdit>("TextEdit").Text = _currentFolder.Files[fileIndex].Item2;

        if (isPasswordProtected == false)
        {
            notepad.Show();
        }
        else if (isPasswordProtected == true)
        {
            GetNode<Control>("PasswordProtected").Show();
            GetNode<Label>("PasswordProtected/WrongPassword").Hide();
        }
    }

//-----------------------------------------------------------------------------
    private void OnGoUpButtonPressed()
    {
        if (_currentFolder.Parent != null)
        {
            _currentFolder = _currentFolder.Parent;

            UpdateScreen();
        }
    }

//-----------------------------------------------------------------------------
    private void OnCloseButtonPressed()
    {
        RequestClose();
    }

//-----------------------------------------------------------------------------
    private void OnNotepadCloseButtonPressed()
    {
        GetNode<Control>("Notepad").Hide();
    }
//-----------------------------------------------------------------------------
    private void OnPasswordProtectedCloseButtonPressed()
    {
        GetNode<Control>("PasswordProtected").Hide();
    }
//-----------------------------------------------------------------------------
    public void OnLineEditTextEntered(String text)
    {
        if (text == "1994")
        {
            GetNode<Control>("Notepad").Show();

            GetNode<LineEdit>("PasswordProtected/LineEdit").Editable = false;
            GetNode<Control>("PasswordProtected").Hide();
        }
        else
        {
            GetNode<Label>("PasswordProtected/WrongPassword").Show();
        }
    }
}
