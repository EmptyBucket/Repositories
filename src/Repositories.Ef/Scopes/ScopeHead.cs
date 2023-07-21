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

namespace Repositories.Scopes;

internal class ScopeHead<TScope> : IScopeHead<TScope> where TScope : IScope<TScope>
{
    private TScope _scope;

    public ScopeHead(TScope scope)
    {
        _scope = scope;
    }

    public TScope Head
    {
        get
        {
            var _ = TryTakeChild(ref _scope) || TryTakeParent(ref _scope);
            return _scope;
        }
    }

    private static bool TryTakeChild(ref TScope scope)
    {
        var isSuccess = false;

        while (scope.Child is { IsDisposed: false })
        {
            scope = scope.Child;
            isSuccess = true;
        }

        return isSuccess;
    }

    private static bool TryTakeParent(ref TScope scope)
    {
        var isSuccess = false;

        while (scope is { IsDisposed: true, Parent: not null })
        {
            scope = scope.Parent;
            isSuccess = true;
        }

        return isSuccess;
    }
}