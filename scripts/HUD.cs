using Godot;

public class HUD : CanvasLayer
{
    [Signal]
    public delegate void GoToLeft();
    [Signal]
    public delegate void GoToRight();

    public override void _Ready()
    {
        PauseMode = Node.PauseModeEnum.Process;
        GetNode<Label>("Control/Pause").Hide();
    }

//-----------------------------------------------------------------------------
    public override void _Input(InputEvent inputEvent)
    {
        //Pause the game
        if (inputEvent.IsActionPressed("pause"))
        {
            GetNode<Label>("Control/Pause").Visible = !GetNode<Label>("Control/Pause").Visible;
            GetTree().Paused = !GetTree().Paused;
            SetMoveButtonEnabled(GetNode<Button>("Control/GoToLeftButton").Disabled);
        }
    }
//-----------------------------------------------------------------------------
    public void SetMoveButtonEnabled(bool enabled)
    {
        GetNode<Button>("Control/GoToLeftButton").Disabled = !enabled;
        GetNode<Button>("Control/GoToRightButton").Disabled = !enabled;
    }

//-----------------------------------------------------------------------------
    public void OnGoToLeftButtonPressed()
    {
        EmitSignal(nameof(GoToLeft));
    }

//-----------------------------------------------------------------------------
    public void OnGoToRightButtonPressed()
    {
        EmitSignal(nameof(GoToRight));
    }
}
