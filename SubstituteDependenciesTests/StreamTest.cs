using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using SubsituteDependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstituteDependenciesTests
{
    public class StreamTest
    {
        [Fact]
        public void MemoryStream()
        {
            using MemoryStream stream = new();
            StreamWriter writer = new(stream);
            writer.WriteLine("abc");
            writer.WriteLine("def");
            writer.WriteLine("ghi");
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            var line = Utils.FindFirstMatchingLine(stream, "f");
            Assert.Equal("def", line);
        }

        [Fact]
        public void FileStream()
        {
            var file = new FileInfo("testdata.txt");
            using var stream = file.OpenRead();

            var line = Utils.FindFirstMatchingLine(stream, "f");
            Assert.Equal("def", line);
        }

        //setting to embed the donotcopy.txt in project assembly is added in the csproj file. check the project file
        [Fact]
        public void EmbeddedResource()
        {
            var type = this.GetType();
            var asm = type.Assembly;
            using var stream = asm.GetManifestResourceStream(type.Namespace + ".dontcopy.txt");

            var line = Utils.FindFirstMatchingLine(stream!, "f");
            Assert.Equal("def", line);
        }

        //Normal strings vs Verbatim strings
        public void StringsExample()
        {
            string bookTitle = ".Net in Action";
            string userId = Guid.NewGuid().ToString();
            string resourceId = "Resource";

            using MemoryStream stream = new();
            StreamWriter writer = new(stream);

            writer.WriteLine("<?xml version=\"1.0\"?>");
            writer.WriteLine("<books>");
            writer.WriteLine("\t<book title=\".NET in Action\"/>");
            writer.WriteLine("</books>");

            writer.Write(@"<?xml version=""1.0""?>
            <books>
              <book title="".NET in Action""/>
            </books>
            ");

            writer.WriteLine(@$"<?xml version=""1.0""?>
            <books>
              <book title=""{bookTitle}""/>
            </books>
            ");

            writer.WriteLine( /*lang=json,strict*/
            @$"{{
              ""RequestParams"": {{
                ""UserId"": ""{userId}"",
                ""ResourceId"": ""{resourceId}""
              }}
            }}");

            var jsonString = /*lang=json,strict*/ """
            {
              "RequestParams": {
                "UserId": "myuserId",
                "ResourceId": "myResourceId"
              }
            }
            """;
        }
    }
}
