using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    public abstract class InstancingGroupBase
    {
        
        protected Mesh instanceMesh;
        protected Material[] instanceMaterials;

        // ✅ Double Buffer
        private ComputeBuffer[] drawDataBuffers = new ComputeBuffer[2];
        private int activeBufferIndex = 0;

        protected ComputeBuffer[] argsBuffer;
        protected uint[][] args;
        protected MaterialPropertyBlock[] mpb;
        protected List<InstanceBase> instances;
        protected Bounds bounds;
        
        public InstancingGroupBase(int subMeshCount, Mesh mesh)
        {
            instanceMesh = mesh;

            argsBuffer = new ComputeBuffer[subMeshCount];
            args = new uint[subMeshCount][];
            mpb = new MaterialPropertyBlock[subMeshCount];
            instances = new List<InstanceBase>();
            
            for (int i = 0; i < subMeshCount; i++)
            {
                args[i] = new uint[5];
                argsBuffer[i] = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);
                mpb[i] = new MaterialPropertyBlock();

                args[i][0] = instanceMesh.GetIndexCount(i);
                args[i][2] = instanceMesh.GetIndexStart(i);
                args[i][3] = instanceMesh.GetBaseVertex(i);
            }
        }


        public abstract void BakeDrawData(out ComputeBuffer buffer);
     
        public void Render()
        {
            if (instances.Count == 0)
            {
                return;
            }

            var currentBuffer = drawDataBuffers[activeBufferIndex];
            if (currentBuffer == null)
                return;

            for (int i = 0; i < instanceMaterials.Length; i++)
            {
                mpb[i].SetBuffer("_DrawData", currentBuffer);

                Graphics.DrawMeshInstancedIndirect(
                    instanceMesh, i, instanceMaterials[i],
                    bounds,
                    argsBuffer[i], 0, mpb[i]
                );
            }
        }

        // ✅ Double Buffer 적용된 버퍼 업데이트
        public void UpdateDrawBuffers()
        {
            if (instances.Count == 0)
            {
                return;
            }

            bounds = new Bounds(Vector3.zero, new Vector3(1000, 1000, 1000));

            int newBufferIndex = 1 - activeBufferIndex;
            
            // 새 버퍼 준비
            if (drawDataBuffers[newBufferIndex] == null || drawDataBuffers[newBufferIndex].count != instances.Count)
            {
                BakeDrawData(out var buffer);
                drawDataBuffers[newBufferIndex]?.Release();
                drawDataBuffers[newBufferIndex] = buffer;
            }

            // 새 데이터 GPU로 업로드
            

            // 인스턴스 수 반영
            for (int i = 0; i < instanceMaterials.Length; i++)
            {
                args[i][1] = (uint)instances.Count;
                argsBuffer[i].SetData(args[i]);
            }

            // 교체
            activeBufferIndex = newBufferIndex;
        }

        public virtual void UpdateCustomData(string tag, object data)
        {
            
        }

        public void AddDrawData(InstanceBase instance) => instances.Add(instance);
        public void RemoveDrawData(InstanceBase instance) => instances.Remove(instance);

        public virtual void Release()
        {
            if (argsBuffer != null)
            {
                for (int i = 0; i < argsBuffer.Length; i++)
                {
                    argsBuffer[i]?.Release();
                    argsBuffer[i] = null;
                }
            }

            for (int i = 0; i < drawDataBuffers.Length; i++)
            {
                drawDataBuffers[i]?.Release();
                drawDataBuffers[i] = null;
            }
        }
    }
}