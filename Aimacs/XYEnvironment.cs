using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Aimacs;

public static class XYEnv 
{
    public enum Type : byte { None = 0, Agent = 1, Wall = 2, Dirt = 3, Gold = 4, }
    public enum Direction : byte { North = 0, South = 1, East = 2, West = 3 }

    private static int _counter = 0;
    private static int NextId() => ++_counter;

	  public readonly struct TypeRef : IComparable<TypeRef>, IEquatable<TypeRef>
    {
        public readonly Type Type;
        public readonly int Id;

        public TypeRef(Type t)
        {
            Type = t;
            Id = NextId();
        }

        public readonly override string ToString() // add Dirt and Gold possibilities
        {
            string name = (Type == Type.Agent) ? "xyagent" : "wall";
            return "[" + name + ", " + Id + "]";
        }

        public readonly int CompareTo(TypeRef other) => Id.CompareTo(other.Id);
        public readonly bool Equals(TypeRef other) => Id == other.Id;
        public readonly override bool Equals(object? obj) => obj is TypeRef o && Equals(o);
        public readonly override int GetHashCode() => Id;
        public static bool operator <(TypeRef a, TypeRef b) => a.Id < b.Id;
        public static bool operator >(TypeRef a, TypeRef b) => a.Id > b.Id;
        public static bool operator ==(TypeRef a, TypeRef b) => a.Id == b.Id;
        public static bool operator !=(TypeRef a, TypeRef b) => a.Id != b.Id;
    }

    public struct Agent
    {
        public bool IsAlive = true;
        public TypeRef Ref { get; set; }

        public Agent()
        {
            Ref = new TypeRef(Type.Agent);
        }
    }

    public struct Wall
    {
        public TypeRef Ref { get; set; }

        public Wall()
        {
            Ref = new TypeRef(Type.Wall);
        }
    }

    public readonly record struct XYLocation
    {
        private readonly int _x; 
        private readonly int _y;

        public int X => _x;
        public int Y => _y;

        public XYLocation(int x, int y)
        {
            if (x <= 0) throw new ArgumentOutOfRangeException(nameof(x));
            if (y <= 0) throw new ArgumentOutOfRangeException(nameof(y));

            _x = x;
            _y = y;
        } 
    }

    public readonly record struct Element(TypeRef T, XYLocation Loc);

    public class XYState
    {
        public int Width { get; }
        public int Height { get; }
        public HashSet<TypeRef> agents;
        public List<Element> elements;
        public Dictionary<TypeRef, XYLocation> locations;

        public XYState(int width, int height)
        {
            Width = width;
            Height = height;
            elements = new List<Element>(width * height);
            agents = [];
            locations = [];
        }
    }

    public static bool AddObjectToLocation(TypeRef obj, XYState state, XYLocation loc)
    {
        CheckLocation(state, loc);

        if (obj.Type == Type.Agent) 
            state.agents.Add(obj);

        state.locations.Add(obj, loc);
        state.elements.Add(new Element(obj, loc));

        return true; 
    }

    public static bool MoveObjectToLocation(TypeRef obj, XYState state, XYLocation loc)
    {
        if (!state.locations.ContainsKey(obj))
            throw new KeyNotFoundException("Object not found.");

        CheckLocation(state, loc);

        state.locations[obj] = loc;
       
        int index = state.elements.FindIndex(e => e.T == obj);
        if (index < 0)
            throw new KeyNotFoundException("Element not found.");

        state.elements[index] = state.elements[index] with { Loc = loc };
        return true;
    }

    public static bool GetObjectsAtLocation(XYState state, XYLocation loc, Span<TypeRef> dest, out int count)
    {
      CheckLocation(state, loc);

      int required = 0;
      for (int i = 0; i < state.elements.Count; i++)
      {
          if (state.elements[i].Loc == loc)
              required++;
      }

      count = required;
      if (required > dest.Length)
          return false;

      int written = 0;
      for (int i = 0; i < state.elements.Count; i++)
      {
          var element = state.elements[i];
          if (element.Loc == loc)
              dest[written++] = element.T;

      }
      return true;
    }

    public static XYState CreateState(int width, int height)
    {
        if (width is < 1 or > 256 || height is < 1 or > 256)
            throw new ArgumentOutOfRangeException("Dimensions of width or height exceed constraints");

        return new XYState(width, height);
    }

    public static TypeRef CreateObject(Type type) 
    {
        var objRef = new TypeRef(type);
        return objRef;
    }

    public static TypeRef CreateAgent()
    {
        return CreateObject(Type.Agent);
    }

    public static TypeRef CreateWall()
    {
        return CreateObject(Type.Wall);
    }

    public static void CheckLocation(XYState state, XYLocation loc)
    {
        if (loc.X <= 0 || loc.X > state.Width || loc.Y <= 0 || loc.Y > state.Height) 
            throw new ArgumentOutOfRangeException(nameof(loc));
    }
    
    public static XYLocation GetLocation(TypeRef obj, XYState state)
    {
        return state.locations[obj];
    }

}





