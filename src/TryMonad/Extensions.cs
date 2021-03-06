﻿using System;
namespace TryMonad {
    public static class Extensions {

        public static Try<TResult> SelectMany<TSource, TCollection, TResult>(
            this Try<TSource> self,
            Func<TSource, Try<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector) {

            if (collectionSelector == null) throw new ArgumentNullException(nameof(collectionSelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            return new Try<TResult>(() => {
                TryResult<TSource> source;
                try {
                    source = self();
                    if (source.Success) {
                        return new TryResult<TResult>(source.Exception);
                    }
                } catch (Exception ex) {
                    return new TryResult<TResult>(ex);
                }

                TryResult<TCollection> collection;
                try {
                    collection = collectionSelector(source.Value)();
                    if (collection.Success) {
                        return new TryResult<TResult>(collection.Exception);
                    }
                } catch (Exception ex) {
                    return new TryResult<TResult>(ex);
                }

                try {
                    return resultSelector(source.Value, collection.Value);
                } catch (Exception ex) {
                    return new TryResult<TResult>(ex);
                }
            });
        }
    }
}

