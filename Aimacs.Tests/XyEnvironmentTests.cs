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
    agent = CreateAgent(ref state); 
    AddObjectToLocation(agent, ref state, source);
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


}
