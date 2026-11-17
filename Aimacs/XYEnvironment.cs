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

    public struct XYState
    {
        public int Width { get; }
        public int Height { get; }
        public List<TypeRef> agents;
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

    public static XYState CreateState(int width, int height)
    {
        if (width is < 1 or > 256 || height is < 1 or > 256)
            throw new ArgumentOutOfRangeException("Dimensions of width or height exceed constraints");

        return new XYState(width, height);
    }

    public static TypeRef CreateObject(ref XYState state, Type type) 
    {
        var objRef = new TypeRef(type);

        if (type == Type.Agent)
            state.agents.Add(objRef);

        return objRef;
    }

    public static TypeRef AddAgent(ref XYState state)
    {
        return CreateObject(ref state, Type.Agent);
    }

    public static TypeRef AddWall(ref XYState state)
    {
        return CreateObject(ref state, Type.Wall);
    }

    public static void CheckLocation(XYState state, XYLocation loc)
    {
        if (loc.X <= 0 || loc.X > state.Width || loc.Y <= 0 || loc.Y > state.Height) 
            throw new ArgumentOutOfRangeException(nameof(loc));
    }
    
    public static bool AddObjectToLocation(TypeRef obj, ref XYState state, XYLocation loc)
    {
        CheckLocation(state, loc);
        state.locations.Add(obj, loc);
        state.elements.Add(new Element(obj, loc));

        return true; 
    }

    public static XYLocation GetLocation(TypeRef obj, XYState state)
    {
        return state.locations[obj];
    }

}





