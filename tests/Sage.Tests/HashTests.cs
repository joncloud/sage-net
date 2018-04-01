using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sage.Tests
{
    public class HashTests
    {
        public static IEnumerable<object[]> JsonHashes
        {
            get => new [] { 
                new HashData(
                    "MD5",
                    "0x9C86CA3802EC778BEBCFF6B1793AF41A", 
                    "0x11FBFA232A95C1A457D9B40B0A80EAE4"
                ),
                new HashData(
                    "SHA1",
                    "0x5707796ED86C4047C81339B816C831BF0CCB7CF1", 
                    "0x66C8B861C1C434FEAA30BCB53327BBF98F12B476"
                ),
                new HashData(
                    "SHA256",
                    "0x91738898BE6EFB426A36452F1542D8125D88BA477181C5050F18AA660B752A62", 
                    "0xD090F0B1DC045D93136B03DBE30DB9F3AB4777D12F512168549B191924C0EE2F"
                ),
                new HashData(
                    "SHA384",
                    "0xE84C9DEC1FFE3F020CF2E2310A671AFFEECE1457D925FC72C55A7A7C50809E5407ACC45034EDC0FBEE8E25308BFE1561", 
                    "0x18E7605B6E5A5F916518A92F05C71FDA7A539CB51E57B71CB5158F1EBB5295D972112B9309D8C6BAC03AE6096D0ECE53"
                ),
                new HashData(
                    "SHA512",
                    "0xAE5917D7F6A8D75A4DBB62DF8FB1FF1271F57FE0A2CFB1CC1E9053CB8ABF346C688CFA2380789DCC77C73D352884DD9119AF2ACE333008C6A0F368A4F160803E", 
                    "0x018600ACC07E68CAAD92C8C2E9735627D2FBAE99CC16C7C0824C1DF26CB885F07346C4446258746E734FF10C3B6668D24D784CB145EF7A89A34C7610ED044C74"
                )
            }.Select(x => new object[] { x });
        }

        [MemberData(nameof(JsonHashes))]
        [Theory]
        public void JsonTests(HashData data)
        {
            Tests(
                () => Program.AsJson(Sql.ConnectionString, hash: data.AlgorithmName),
                data.OneValue,
                data.TwoValue
            );
        }
        public static IEnumerable<object[]> TabHashes
        {
            get => new [] { 
                new HashData(
                    "MD5",
                    "0xF851F5BA5DEB579BBFE5D98E9CD268F6", 
                    "0x048F9E6DEF2045421BC057264C16A042"
                ),
                new HashData(
                    "SHA1",
                    "0x6325FB77195473E20378FD0655476C91D4DD7B7C", 
                    "0x4ADBDBFFB780B672F0B5A8CA3096EE7EACB02C7A"
                ),
                new HashData(
                    "SHA256",
                    "0x313EA196881D370AEEAF78E274B0D08541F6CBF0DDFC7BE57A4594AD0A752A5C", 
                    "0x54CB67D1746CD42CA947F6CE705060D0FB5540E55D588F5726CDAD0B73F41618"
                ),
                new HashData(
                    "SHA384",
                    "0x55F056AE4F363D17D5AD2541E32FE2E5C84F0683C68D473D918F2DE69B013F23B8A9C263A4AEFAEB764871A58D51BBF1", 
                    "0x4C79E924D9251DC4B2635A9E83DAF19D2973F20BB93531810A70C663C39F5DB800A5B0B0393A8D6EC50401980CC826EF"
                ),
                new HashData(
                    "SHA512",
                    "0x6431D498C48B6FA8E59371341CDDB492E397880073A5E1C74127C8032D8E29877DCD7A81F91E9E99D44C7D1569F4724868C122BF633D7FBDCD24E5478A6CF415", 
                    "0x7B6192AF485A3E4515BB75B3DF6B8A3D6CBC4AC61F56D7C47434EFC7B8714220DCBC3B671FFE57FC909F44D17F692207397F9F8DDB507887361C42D6EA3F33A0"
                )
            }.Select(x => new object[] { x });
        }

        [MemberData(nameof(TabHashes))]
        [Theory]
        public void TabTests(HashData data)
        {
            Tests(
                () => Program.AsTabbed(Sql.ConnectionString, hash: data.AlgorithmName),
                data.OneValue,
                data.TwoValue
            );
        }

        public class HashData
        {
            public string AlgorithmName { get; }
            public string OneValue { get; }
            public string TwoValue { get; }

            public HashData(string algorithmName, string oneValue, string twoValue)
            {
                AlgorithmName = algorithmName;
                OneValue = oneValue;
                TwoValue = twoValue;
            }
        }

        static void Tests(Func<int> fn, string hash1, string hash2)
        {
            var queries = new[] {
                new Query
                {
                    Name = "Query1",
                    CommandText = "SELECT 1 [Num]"
                },
                new Query
                {
                    Name = "Query2",
                    CommandText = "SELECT 2 [Num]"
                }
            };
            var (exitCode, stdOut, __) = TestHarness.Run(fn, queries);
            Assert.Equal(0, exitCode);
            var actual = stdOut.Split(Environment.NewLine)
                .Where(IsNotEmpty)
                .Select(AsHash)
                .ToArray();
            var expected = new[]
            {
                new Hash("Query1", hash1),
                new Hash("Query2", hash2)
            };

            Assert.Equal(expected, actual);

            bool IsNotEmpty(string s) => !string.IsNullOrWhiteSpace(s);

            Hash AsHash(string s)
            {
                string[] parts = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return new Hash(parts[0], parts[1]);
            }
        }
    }
}
