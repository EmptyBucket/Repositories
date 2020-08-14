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

using Microsoft.EntityFrameworkCore;

namespace Repositories.Scopes;

internal interface IDbContextScope : IScope<IDbContextScope>
{
	DbContext DbContext { get; }
}

internal interface IDbContextScope<out TDbContext> : IDbContextScope, IScope<IDbContextScope<TDbContext>>
	where TDbContext : DbContext
{
	new TDbContext DbContext { get; }

	DbContext IDbContextScope.DbContext => DbContext;

	new IDbContextScope<TDbContext>? Parent { get; }

	IDbContextScope? IScope<IDbContextScope>.Parent => Parent;

	new IDbContextScope<TDbContext>? Child { get; }

	IDbContextScope? IScope<IDbContextScope>.Child => Child;

	new bool Disposed { get; }

	bool IScope<IDbContextScope>.Disposed => Disposed;

	new IDbContextScope<TDbContext> Begin();

	IDbContextScope IScope<IDbContextScope>.Begin() => Begin();
}