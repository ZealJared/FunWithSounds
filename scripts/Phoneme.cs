using Godot;
using System.Collections.Generic;

public class Phoneme : AnimatedSprite
{
  [Export]
  public Godot.Collections.Array<NodePath> relatedGraphemePaths = new Godot.Collections.Array<NodePath>();
  [Export]
  public AudioStreamOGGVorbis sound;
  private List<Grapheme> relatedGraphemes = new List<Grapheme>();
  private List<Grapheme> selectedGraphemesCorrect = new List<Grapheme>();
  private List<Grapheme> selectedGraphemesIncorrect = new List<Grapheme>();
  [Export]
  public bool enabled = true;
  private bool permanentlyDisabled = false;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    foreach (NodePath graphemePath in this.relatedGraphemePaths)
    {
      Grapheme grapheme = null;
      grapheme = GetNode<Grapheme>(graphemePath);
      if (grapheme is Grapheme realGrapheme)
      {
        this.relatedGraphemes.Add(realGrapheme);
      }
      else
      {
        GD.Print("Invalid RelatedGraphemes for: " + this.Name);
      }
    }
    if (!this.enabled)
    {
      this.Frame = 1;
      this.permanentlyDisabled = true;
    }
  }

  public void AcceptDrop(Grapheme grapheme)
  {
    if (!this.enabled)
    {
      return;
    }
    GD.Print(this.Name);
    if (this.IsRelatedTo(grapheme))
    {
      this.selectedGraphemesCorrect.Add(grapheme);
    }
    else
    {
      this.selectedGraphemesIncorrect.Add(grapheme);
    }
    this.GetParent<Main>().SetActivePhoneme(this);
  }

  public void GetResult()
  {
    List<Grapheme> graphemesMissing = new List<Grapheme>();
    foreach (Grapheme grapheme in this.relatedGraphemes)
    {
      if (!this.selectedGraphemesCorrect.Contains(grapheme))
      {
        graphemesMissing.Add(grapheme);
      }
    }
    GD.Print("Correct:");
    GD.Print(string.Join(", ", this.selectedGraphemesCorrect));
    foreach (Grapheme correctGrapheme in this.selectedGraphemesCorrect)
    {
      correctGrapheme.MarkCorrect();
    }
    GD.Print("Inorrect:");
    GD.Print(string.Join(", ", this.selectedGraphemesIncorrect));
    foreach (Grapheme incorrectGrapheme in this.selectedGraphemesIncorrect)
    {
      incorrectGrapheme.MarkIncorrect();
    }
    GD.Print("Missing:");
    GD.Print(string.Join(", ", graphemesMissing));
    foreach (Grapheme missingGrapheme in graphemesMissing)
    {
      missingGrapheme.MarkMissing();
    }
    if (this.selectedGraphemesIncorrect.Count == 0 && graphemesMissing.Count == 0)
    {
      GD.Print("100% Correct!");
    }
  }

  private bool IsRelatedTo(Grapheme grapheme)
  {
    if (this.relatedGraphemes.Contains(grapheme))
    {
      return true;
    }
    return false;
  }

  public void Reset()
  {
    if(this.permanentlyDisabled)
    {
      return;
    }
    this.Frame = 0;
    this.selectedGraphemesCorrect.Clear();
    this.selectedGraphemesIncorrect.Clear();
  }

  public void Disable()
  {
    if (!this.enabled)
    {
      return;
    }
    this.enabled = false;
    this.Frame = 1;
  }

  public void Enable()
  {
    if (this.enabled || this.permanentlyDisabled)
    {
      return;
    }
    this.enabled = true;
    this.Frame = 0;
  }

  public void MarkIncorrect()
  {
    this.Frame = 3;
  }

  public void MarkCorrect()
  {
    this.Frame = 2;
  }

  //  // Called every frame. 'delta' is the elapsed time since the previous frame.
  //  public override void _Process(float delta)
  //  {
  //      
  //  }
}
