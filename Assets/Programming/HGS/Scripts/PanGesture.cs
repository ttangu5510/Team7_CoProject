using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace SHG {
  public record PanGesture {
    public enum State {
      Panning,
      Ended
    }
    public State CurrentState;
    public Vector2 StartPosition => this.Positions[0];
    public List<Vector2> Positions;
    public List<float> TimeStamps;
    public Vector2 EndPosition => this.Positions[this.Positions.Count - 1];
    public float StartTime => this.TimeStamps[0];
    public float EndTime => this.TimeStamps[this.TimeStamps.Count - 1];

    public void AddPosition(in Vector2 pos) {
      this.Positions.Add(pos);
      this.TimeStamps.Add(Time.time);
    }

    public override string ToString() {
      var builder = new StringBuilder();
      builder.Append($"[{nameof(PanGesture)}; ");
      builder.Append($"{nameof(CurrentState)}: {this.CurrentState}; ");
      builder.Append($"{nameof(StartPosition)}: {this.StartPosition}; ");
      builder.Append($"{nameof(EndPosition)}: {this.EndPosition}; ");
      builder.Append($"{nameof(StartTime)}: {this.StartTime}; ");
      builder.Append($"{nameof(EndTime)}: {this.EndTime}; ");
      builder.Append($" {nameof(Positions)}: {this.Positions.Count}; ]");
      return (builder.ToString());
    }

    public static PanGesture Create() {
      var gesture = new PanGesture {
        CurrentState = PanGesture.State.Panning,
        Positions = new List<Vector2>(),
        TimeStamps = new List<float>(),
      };
      return (gesture);
    }

    public PanGesture EndGesture() {
      this.CurrentState = PanGesture.State.Ended;
      return (this);
    }
  }
}
