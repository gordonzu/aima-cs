namespace Aimacs.Tests;

using static Aimacs.XYEnv;

public class XYEnvironmentTests
{
    private TypeRef agent;
    private XYState state;
    private readonly XYLocation source, destination; 

    public XYEnvironmentTests() 
    {
        source = new XYLocation(2, 2);
        destination = new XYLocation(3, 3);

        state = CreateState(10, 12);
        agent = CreateAgent(); 
        AddObjectToLocation(agent, state, source);
    }

    [Fact]
    public void TestMakeEnvironment()
    {
        Assert.Equal(10, state.Width);
        Assert.Equal(12, state.Height);
        Assert.Equal(120, state.elements.Capacity);
    }

    [Theory]
	[InlineData(0, 5)]
	[InlineData(5, 0)]
	[InlineData(-1, 5)]
    public void TestEnvironmentRejectsNonPositiveParams(int w, int h)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateState(w, h));
    }

    [Fact]
    public void TestAddObject()
    {
        Assert.Single(state.agents);
        Assert.Equal(new XYLocation(2, 2), GetLocation(agent, state));
    }

    [Fact]
    public void TestAddObject2()
    {
        Span<TypeRef> buf = stackalloc TypeRef[16];
        var wall = CreateWall();

        Assert.True(AddObjectToLocation(wall, state, new XYLocation(9, 9)));
        Assert.Single(state.agents);
        Assert.Equal(2, state.elements.Count);
        Assert.True(GetObjectsAtLocation(state, new XYLocation(9, 9), buf, out int count));
        Assert.Equal(1, count);
    }

    [Fact]
    public void TestAddObjectTwice()
    {
        var a = CreateAgent();

        Assert.Single(state.agents);
        Assert.True(AddObjectToLocation(a, state, new XYLocation(5, 5)));
        Assert.Equal(2, state.agents.Count);
        Assert.Equal(new XYLocation(5, 5), GetLocation(a, state));
    }

    [Fact]
    public void TestMoveObjectToAbsoluteLocation()
    {
        var loc = new XYLocation(5, 5);

        Assert.True(MoveObjectToLocation(agent, state, loc));
        Assert.Equal(loc, GetLocation(agent, state));
        Assert.NotEqual(source, GetLocation(agent, state));
    }

    [Fact]
    public void TestAddAndMoveFailsWhenBlockedByWall()
    {
        var a = CreateAgent();
        Assert.True(AddObjectToLocation(CreateWall(), state, destination));
        Assert.False(AddObjectToLocation(a, state, destination));
    }

    [Fact]
    public void TestAddAndMoveSucceedWhenSharingLocationWithNonWall()
    {
        var a = CreateAgent();

    }

    [Fact]
    public void TestMoveObject()
    {
        var loc = new XYLocation(5, 5);

        Assert.True(MoveObjectToLocation(agent, state, loc));
        Assert.Equal(loc, GetLocation(agent, state));
        MoveObject(agent, state, Direction.North);
        //Assert.Equal(new XYLocation(5, 6), GetLocation(agent, state));

    }









}
