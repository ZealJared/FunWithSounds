using Godot;
using System.Collections.Generic;

public class Main : Node2D
{
  List<Phoneme> phonemes = new List<Phoneme>();
  List<Grapheme> graphemes = new List<Grapheme>();
  Node activeDragNode;
  Phoneme activePhoneme = null;
  int questionTargetPhonemeIndex = 0;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();
    randomNumberGenerator.Randomize();
    this.activeDragNode = this;
    foreach (Node child in this.GetChildren())
    {
      if (child is Phoneme phoneme && phoneme.enabled)
      {
        this.phonemes.Add(phoneme);
      }
      if (child is Grapheme grapheme)
      {
        this.graphemes.Add(grapheme);
      }
    }
    this.phonemes.Sort(Comparer<Phoneme>.Create((a, b) => randomNumberGenerator.RandiRange(-1, 1)));
    this.Announce(this.GetQuestionTargetPhoneme());
    GD.Print(this.GetQuestionTargetPhoneme().Name);
  }

  Phoneme GetQuestionTargetPhoneme()
  {
    return this.phonemes[this.questionTargetPhonemeIndex];
  }

  public void NextQuestion()
  {
    foreach (Grapheme grapheme in this.graphemes)
    {
      grapheme.Reset();
    }
    foreach (Phoneme phoneme in this.phonemes)
    {
      phoneme.Reset();
    }
    this.GetQuestionTargetPhoneme().Reset();
    this.questionTargetPhonemeIndex++;
    if (this.questionTargetPhonemeIndex == this.phonemes.Count)
    {
      GetTree().QueueDelete(GetTree().Root.GetChild(0));
      GetTree().Root.AddChild(GD.Load<PackedScene>("res://final.tscn").Instance());
      return;
    }
    this.Announce(this.GetQuestionTargetPhoneme());
    this.HideNextButton();
    this.UnsetActivePhoneme();
  }

  void Announce(Phoneme phoneme)
  {
    AudioStreamPlayer player = GetNode<AudioStreamPlayer>(new NodePath("AudioStreamPlayer"));
    player.Stream = this.GetQuestionTargetPhoneme().sound;
    player.Play();
  }

  public override void _Input(InputEvent @event)
  {
    if (@event is InputEventMouseMotion eventMouseMotion && this.activeDragNode != this && this.activeDragNode is Grapheme dragNode)
    {
      dragNode.Translate(eventMouseMotion.Relative);
    }
    if (@event is InputEventMouseButton eventMouseButton && this.activeDragNode == this && eventMouseButton.Pressed == true && eventMouseButton.ButtonIndex == 1)
    {
      Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;
      Godot.Collections.Array graphemeCheck = spaceState.IntersectPoint(eventMouseButton.Position, maxResults: 1, collideWithAreas: true, collisionLayer: 2);
      if (graphemeCheck.Count > 0)
      {
        Area2D graphemeCollider = (Area2D)((Godot.Collections.Dictionary)graphemeCheck[0])["collider"];
        Grapheme grapheme = graphemeCollider.GetParent<Grapheme>();
        this.Drag(grapheme);
      }
    }
    if (@event is InputEventMouseButton eventMouseButtonRelease && this.activeDragNode != this && eventMouseButtonRelease.Pressed == false && eventMouseButtonRelease.ButtonIndex == 1)
    {
      this.Drop(eventMouseButtonRelease.Position);
    }
  }

  public void Drag(Grapheme node)
  {
    this.activeDragNode = node;
    node.DragStart();
  }

  public void Drop(Vector2 mousePosition)
  {
    if (this.activeDragNode == this)
    {
      return;
    }
    else if (this.activeDragNode is Grapheme dragNode)
    {
      this.HandleDropAt(mousePosition);
      dragNode.DragStop();
      this.activeDragNode = this;
    }
  }

  private void HandleDropAt(Vector2 mousePosition)
  {
    if (this.activeDragNode is Grapheme dragNode)
    {
      Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;
      Godot.Collections.Array phonemeCheck = spaceState.IntersectPoint(mousePosition, maxResults: 1, collideWithAreas: true, collisionLayer: 1);
      if (phonemeCheck.Count > 0)
      {
        Area2D phonemeCollider = (Area2D)((Godot.Collections.Dictionary)phonemeCheck[0])["collider"];
        Phoneme phoneme = phonemeCollider.GetParent<Phoneme>();
        phoneme.AcceptDrop(dragNode);
      }
    }
  }

  public void SetActivePhoneme(Phoneme phoneme)
  {
    this.activePhoneme = phoneme;
    this.HideInactivePhonemes();
  }

  public void HideInactivePhonemes()
  {
    Godot.Collections.Array children = this.GetChildren();
    foreach (Node child in children)
    {
      if (child is Phoneme phoneme)
      {
        phoneme.Disable();
      }
    }
    this.activePhoneme.Enable();
  }

  public void UnsetActivePhoneme()
  {
    Godot.Collections.Array children = this.GetChildren();
    foreach (Node child in children)
    {
      if (child is Phoneme phoneme)
      {
        phoneme.Enable();
      }
    }
    this.activePhoneme = null;
  }

  public void GetResult()
  {
    if (!(this.activePhoneme is Phoneme))
    {
      this.activePhoneme = this.GetQuestionTargetPhoneme();
    }
    if (this.activePhoneme != this.GetQuestionTargetPhoneme())
    {
      this.WrongPhoneme();
      return;
    }
    else
    {
      this.RightPhoneme();
    }
    this.activePhoneme.GetResult();
    this.ShowNextButton();
  }

  private void RightPhoneme()
  {
    this.activePhoneme.MarkCorrect();
  }

  private void WrongPhoneme()
  {
    this.activePhoneme.MarkIncorrect();
  }

  public void Reset()
  {
    this.Announce(this.GetQuestionTargetPhoneme());
    if (!(this.activePhoneme is Phoneme phoneme))
    {
      return;
    }
    this.activePhoneme.Reset();
    this.UnsetActivePhoneme();
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
  private void HideNextButton()
  {
    GetNode<NextButton>(new NodePath("NextButton")).Hide();
  }
  private void ShowNextButton()
  {
    GetNode<NextButton>(new NodePath("NextButton")).Show();
  }
}
