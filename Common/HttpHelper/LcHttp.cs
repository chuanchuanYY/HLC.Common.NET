using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpHelper
{
	public class LcHttp:BaseHttp
	{

		#region  构造函数可选配置
		public LcHttp()
        {
			
		}
		public LcHttp(string baseUrl,double timeOut,Action<HttpClient> config)
		{ 
			baseUrl = baseUrl ?? string.Empty;	
			this.TimeOut = timeOut>=0?timeOut:100000;
		    this.Configuration= config;	
		}

		#endregion
		private HttpResponseMessage? _response;
		private event Action<HttpResponseMessage> onResponsed;
		public LcHttp then(Action<HttpResponseMessage> response) {

			onResponsed += res => {
				if (_response!.StatusCode == System.Net.HttpStatusCode.OK)
					response?.Invoke(_response);
			};
			return this;
		}

		public LcHttp Catch(Action<HttpResponseMessage> error)
		{
			onResponsed += res =>
			{
				if (_response!.StatusCode != System.Net.HttpStatusCode.OK)
					error?.Invoke(_response);
			};
			return this;
		}
		public LcHttp Finlly(Action action)
		{
			action?.Invoke();
			return this;
		}
		#region 拦截器

		public void  RequstInterceptor(Action<HttpClient> config,Action<Exception> error=null!)
		{
			this.Configuration += config;
			if(error!=null)
			this.OnError += error;
		}

		public void ResponseInterceptor(Action<HttpContent> response,
			Action<HttpResponseMessage> error =null!)
		{
			then(res =>
			{
				response.Invoke(res.Content);
			});
			Catch((err) => {
				if (error != null)
					error.Invoke(err);
			});
		}
		#endregion
		#region Get 

		public new  LcHttp Get(string url)
		{
			try
			{
				_response = base.Get(url);
				onResponsed.Invoke(_response);
				return this;
			}
			catch (Exception ex)
			{

				this.OnError?.Invoke(ex);
				return this;
			}
			
		   
		}
		public new LcHttp Get(string url,Dictionary<string,object> _params)
		{
			 StringBuilder stringBuilder = new StringBuilder();
			 var keys=_params.Keys;
			foreach (var key in keys)
			{
				stringBuilder.Append($"{key}={_params[key]}&");
			}
			stringBuilder.Remove(stringBuilder.Length-1,1);
			try
			{
				_response = base.Get($"{url}?{stringBuilder.ToString()}");
				onResponsed.Invoke(_response);
				return this;
			}
			catch (Exception ex)
			{
				this.OnError?.Invoke(ex);
				return this;
			}
			
		}
		#endregion

		#region Post
		public new LcHttp Post(string url, object? data = null)
		{
			try
			{
				_response = base.Post(url, data);
				onResponsed.Invoke(_response);
				return this;
			}
			catch (Exception ex)
			{
				this.OnError?.Invoke(ex);
				return this;
			}
		
		}
		#endregion


	}
}
