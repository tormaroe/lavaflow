using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using System.Threading;

namespace LavaFlow.Test.ClientTests
{
    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        [Ignore("Need to make this runnable with in-process and in-memory server")]
        public void Run()
        {
            using (var client = new LavaFlowClient("localhost"))
            {
                string test_aggregate = Guid.NewGuid().ToString("N");

                /** Testing API before aggregate exist **/

                client.GetAggregatesAsync().Result.Should().NotContain(test_aggregate);

                client.GetKeysAsync(test_aggregate).Result.Should().BeEmpty();

                client.GetEventsAsync(test_aggregate, "foo").Result.Should().BeEmpty(); 

                /** Post event **/

                client.PostEventAsync(test_aggregate, "key_one", "data 1")
                    .Result.Should().BeTrue();

                client.GetAggregatesAsync().Result.Should().Contain(test_aggregate);

                var db_keys = client.GetKeysAsync(test_aggregate).Result;
                db_keys.Should().HaveCount(1);
                db_keys.Should().Contain("key_one");

                var db_events = client.GetEventsAsync(test_aggregate, "key_one").Result;
                db_events.Should().HaveCount(1);
                db_events.Should().Contain("data 1");

                /** Multiple events **/

                var tasks = new Task<bool>[100];
                for (int i = 0; i < tasks.Length; i++)
			    {
                    tasks[i] = client.PostEventAsync(test_aggregate, "key_two", "event " + i);
			    }

                Task.WaitAll(tasks);

                db_keys = client.GetKeysAsync(test_aggregate).Result;
                db_keys.Should().HaveCount(2);
                db_keys.Should().Contain("key_two");

                // Eventual consistency ... so what is the value of eventual?
                Thread.Sleep(200);

                db_events = client.GetEventsAsync(test_aggregate, "key_two").Result;
                db_events.Should().HaveCount(100);
            }
        }
    }
}
