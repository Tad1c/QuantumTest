﻿#if (QUANTUM_ADDRESSABLES || QUANTUM_ENABLE_ADDRESSABLES) && !QUANTUM_DISABLE_ADDRESSABLES
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class QuantumPrefabAsset_Addressable : QuantumPrefabAsset {
  public AssetReferenceGameObject Address;

  protected override void Load(in LoadContext context) {
    Debug.Assert(!Address.OperationHandle.IsValid());
    var op = Address.LoadAssetAsync();
    if (op.IsDone) {
      context.Loaded(op.Result);
    } else {
#if (QUANTUM_ADDRESSABLES_USE_WAIT_FOR_COMPLETION || QUANTUM_ENABLE_ADDRESSABLES_WAIT_FOR_COMPLETION) && !QUANTUM_DISABLE_ADDRESSABLES_WAIT_FOR_COMPLETION
      if (context.PreferAsync) 
#endif
      {
        var c = context;
        op.Completed += (_op) => {
          c.Loaded(_op.Result);
        };
      }
#if (QUANTUM_ADDRESSABLES_USE_WAIT_FOR_COMPLETION || QUANTUM_ENABLE_ADDRESSABLES_WAIT_FOR_COMPLETION) && !QUANTUM_DISABLE_ADDRESSABLES_WAIT_FOR_COMPLETION
      else {
        var result = op.WaitForCompletion();
        context.Loaded(result);
      }
#endif
    }
  }

  protected override void Unload() {
    Address.ReleaseAsset();
  }
}
#endif