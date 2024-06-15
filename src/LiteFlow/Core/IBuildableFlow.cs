﻿namespace LiteFlow.Core;

public interface IBuildableFlow<in TIn, TOut>
{
    IFlow<TIn, TOut> Build();
}