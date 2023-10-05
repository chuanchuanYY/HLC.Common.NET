using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpHelper
{
	public class BaseHttp 
	{
        public  string? BaseUrl { get; set; }
		public Double TimeOut { get; set; } = 100000;
		protected Action<HttpClient>? Configuration { get; set; }
		protected Action<Exception>? OnError { get; set; }
		public AuthenticationHeaderValue? Authentication { get; set; }	
		async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
		{
			using var client = new HttpClient();
			Configuration?.Invoke(client);
			client.DefaultRequestHeaders.Authorization = Authentication;
			CancellationTokenSource cancellationToken = new CancellationTokenSource();
			cancellationToken.CancelAfter(TimeSpan.FromMilliseconds(TimeOut));
			return await client.SendAsync(requestMessage, cancellationToken.Token);
		}
		HttpResponseMessage Send(HttpRequestMessage requestMessage)
		{
			using var client = new HttpClient();
			Configuration?.Invoke(client);
			client.DefaultRequestHeaders.Authorization = Authentication;
			CancellationTokenSource cancellationToken = new CancellationTokenSource();
			cancellationToken.CancelAfter(TimeSpan.FromMilliseconds(TimeOut));
			return client.Send(requestMessage, cancellationToken.Token);
		}
		#region Get
		protected async Task<HttpResponseMessage>  GetAsync(string url)
		{
			using HttpRequestMessage requestMessage = new HttpRequestMessage();
			requestMessage.Method = HttpMethod.Get;
			requestMessage.RequestUri = new Uri(BaseUrl+url);
			return await SendAsync(requestMessage);
		}
		protected HttpResponseMessage Get(string url)
		{
			using HttpRequestMessage requestMessage = new HttpRequestMessage();
			requestMessage.Method = HttpMethod.Get;
			requestMessage.RequestUri = new Uri(BaseUrl + url);
			return  Send(requestMessage);
		}
		#endregion
		#region Post
		protected async Task<HttpResponseMessage> PostAsync(string url,object? data=null)
		{
			using HttpRequestMessage requestMessage = new HttpRequestMessage();
			requestMessage.Method = HttpMethod.Post;
			requestMessage.RequestUri = new Uri(BaseUrl + url);
			requestMessage.Content = new StringContent(JsonSerializer.Serialize(data==null?"":data)
				, Encoding.UTF8, "application/json");
			return await SendAsync(requestMessage);	
		}
		protected HttpResponseMessage Post(string url, object? data = null)
		{
			using HttpRequestMessage requestMessage = new HttpRequestMessage();
			requestMessage.Method = HttpMethod.Post;
			requestMessage.RequestUri = new Uri(BaseUrl + url);
			requestMessage.Content = new StringContent(JsonSerializer.Serialize(data == null ? "" : data)
				,Encoding.UTF8,"application/json");//这个后面可以给配置
			return Send(requestMessage);
		}
		#endregion
		protected void Delete()
		{
			
		}
		protected void Put()
		{
			throw new NotImplementedException();
		}
	}
}
