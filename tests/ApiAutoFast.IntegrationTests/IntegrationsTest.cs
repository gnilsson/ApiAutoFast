using Xunit;
using ApiAutoFast;

namespace ApiAutoFast.IntegrationTests;

//[AutoFastEndpoints]
public class TestEntity
{

}

public class Tests
{
    [Theory]
    [InlineData(1)]
    public void Hm(int a)
    {
        Assert.Equal(1, a);
    }
}
