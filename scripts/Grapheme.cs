using Godot;
using System;

public class Grapheme : AnimatedSprite
{
	private int previousZindex;
	private Vector2 previousPosition;
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	private Main GetMain()
	{
		return this.GetParent<Main>();
	}

	public override string ToString()
	{
		return this.Name;
	}

	public void DragStart()
	{
		this.previousZindex = this.ZIndex;
		this.previousPosition = this.Position;
		this.ZIndex = 10;
	}

	public void DragStop()
	{
		this.ZIndex = this.previousZindex;
		this.Position = this.previousPosition;
	}

  public void MarkIncorrect()
  {
    this.Frame = 3;
  }

  public void MarkMissing()
  {
    this.Frame = 4;
  }

  public void MarkCorrect()
  {
    this.Frame = 2;
  }

	public void Reset()
	{
		this.Frame = 0;
	}

  //  // Called every frame. 'delta' is the elapsed time since the previous frame.
  //  public override void _Process(float delta)
  //  {
  //      
  

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
