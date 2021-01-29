using Godot;
using System;

public class Intro : Node2D
{
  ResourceInteractiveLoader loader;
  Label loadingLabel;
  // Declare member variables here. Examples:
  // private int a = 2;
  // private string b = "text";

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {

  }

  public void LoadScene(string path)
  {
    this.loader = ResourceLoader.LoadInteractive(path);
    SetProcess(true);
    GetNode<Control>(new NodePath("Control")).Hide();
    GetNode<CenterContainer>(new NodePath("CenterContainer")).Show();
    this.loadingLabel = GetNode<Label>(new NodePath("CenterContainer/LoadingLabel"));
  }

  public override void _Process(float delta)
  {
    if (this.loader == null)
    {
      SetProcess(false);
      return;
    }
    Error error = this.loader.Poll();
    this.loadingLabel.Text = loader.GetStage() + " / " + loader.GetStageCount();
    if (error == Error.FileEof)
    {
      GetTree().QueueDelete(GetTree().Root.GetChild(0));
      GetTree().Root.AddChild(((PackedScene)this.loader.GetResource()).Instance());
      this.loader = null;
    }
  }
  //  // Called every frame. 'delta' is the elapsed time since the previous frame.
  //  public override void _Process(float delta)
  //  {
  //      
  //  }
}
