// MIT License
// 
// Copyright (c) 2020 Alexey Politov
// https://github.com/EmptyBucket/Repositories
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Repositories.Abstractions.Scopes;

namespace Repositories.Scopes;

internal class UnitOfWork : IUnitOfWork
{
	private readonly IEnumerable<IScopeHead<IDbContextScope>> _scopeHeads;

	public UnitOfWork(IEnumerable<IScopeHead<IDbContextScope>> scopeHeads)
	{
		_scopeHeads = scopeHeads;
	}

	public async Task<IUnitOfWork> RunAsync(Func<Task> func)
	{
		var scopes = new List<IDbContextScope>();

		try
		{
			scopes.AddRange(_scopeHeads.Select(h => h.Head.Begin()));
			await func();
		}
		finally
		{
			await Task.WhenAll(scopes.Select(s => s.DisposeAsync().AsTask())).ConfigureAwait(false);
		}

		return this;
	}
}