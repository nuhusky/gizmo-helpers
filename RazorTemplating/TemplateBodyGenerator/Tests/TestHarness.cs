using Gizmo.RazorTemplating.Samples.Models;
using Gizmo.RazorTemplating.TemplateBodyGenerator;
using NUnit.Framework;


namespace Sizzle.TemplateBodyGenerator.Tests
{
    [TestFixture]
    public class TestHarness
    {
        [Test]
        public void TemplateBodyRenderTest()
        {
            //Arrange
            SampleModel model = new SampleModel();
            model.Message = "Hello, World!";
           
            //Act
            string output = TemplateGenerator.Current.Render(model);
            
            //Assert
            Assert.IsTrue(output.Contains("Hello, World!"));
        }


        [Test]
        public void TemplateBodyRenderWithFooterTest()
        {
            //Arrange
            SampleModel model = new SampleModel();
            model.Message = "Hello, World!";
            model.Footer = new FooterModel()
                               {
                                   Text = "I'm a footer"
                               };

            //Act
            string output = TemplateGenerator.Current.Render(model);

            //Assert
            Assert.IsTrue(output.Contains("I'm a footer"));
        }
    }
}