using Xunit;
using ApiAutoFast;

namespace ApiAutoFast.IntegrationTests;

//[AutoFastEndpoints]
public class Hej
{

}

public class Tests
{
    [Theory]
    [InlineData(1)]
    public void Hm(int a)
    {
        //var ba = new AutoFastDbContext(null);
  //      a.enti
//        var f = new DemoRequest();
      //  var ah = new DemoEndpoint();
        Assert.Equal(1, a);
    }
}
