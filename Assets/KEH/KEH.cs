using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KEH : MonoBehaviour
{
    [Header("블록 생성 설정")]
    public GameObject blockPrefab;
    public Vector3 targetBlockSize = new Vector3(1f, 1f, 1f);
    public int width = 50;
    public int height = 50;
    public float spacing = 0f;

    [Header("생성 옵션")]
    public bool combineMeshes = true; // ? 메쉬 병합 여부 옵션

    void Start()
    {
        //if (Application.isPlaying && blockPrefab != null)
        //{
        //    GenerateGround();
        //}
    }

    public Vector3 GetScaleToFit(GameObject prefab, Vector3 targetSize)
    {
        GameObject temp = MonoBehaviour.Instantiate(prefab);
        Renderer rend = temp.GetComponentInChildren<Renderer>();

        if (rend == null)
        {
            MonoBehaviour.DestroyImmediate(temp);
            return Vector3.one;
        }

        Vector3 originalSize = rend.bounds.size;
        MonoBehaviour.DestroyImmediate(temp);

        if (originalSize == Vector3.zero)
            return Vector3.one;

        float yScale = originalSize.y < 0.01f ? 1f : targetSize.y / originalSize.y;
        return new Vector3(targetSize.x / originalSize.x, yScale, targetSize.z / originalSize.z);
    }

    /// <summary>
    /// blockPrefab을 이용해 width x height 크기의 평지를 생성합니다.
    /// (0,0,0)을 중심으로 생성되며 부모 위치는 변경되지 않습니다.
    /// </summary>
    public void GenerateGround()
    {
        ClearGround();

        if (blockPrefab == null)
        {
            Debug.LogWarning("?? blockPrefab이 설정되지 않았습니다!");
            return;
        }

        Vector3 scale = GetScaleToFit(blockPrefab, targetBlockSize);
        float baseY = transform.position.y;

        // 전체 평지의 중심 오프셋 계산
        Vector3 centerOffset = new Vector3(
            (width - 1) * (targetBlockSize.x + spacing) / 2f,
            0,
            (height - 1) * (targetBlockSize.z + spacing) / 2f
        );

        List<MeshFilter> meshFilters = new List<MeshFilter>();
        List<GameObject> createdBlocks = new List<GameObject>(); // ? 생성된 블록 추적용

        // 블록 생성
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(
                    x * (targetBlockSize.x + spacing),
                    baseY,
                    z * (targetBlockSize.z + spacing)
                );

                pos -= centerOffset;

                GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
                block.transform.localScale = scale;

                createdBlocks.Add(block);

                MeshFilter mf = block.GetComponentInChildren<MeshFilter>();
                if (mf != null)
                    meshFilters.Add(mf);
            }
        }

        // ? 메쉬 병합 처리
        if (combineMeshes && meshFilters.Count > 0)
        {
            CombineGroundMeshes(meshFilters, createdBlocks);
        }
    }

    /// <summary>
    /// 여러 블록의 메쉬를 하나로 병합합니다.
    /// </summary>
    private void CombineGroundMeshes(List<MeshFilter> meshFilters, List<GameObject> originalBlocks)
    {
        CombineInstance[] combine = new CombineInstance[meshFilters.Count];

        for (int i = 0; i < meshFilters.Count; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        // 새 병합 메쉬 오브젝트 생성
        GameObject combined = new GameObject("CombinedGround");
        combined.transform.SetParent(transform, false);

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // 65k 이상 지원
        combinedMesh.CombineMeshes(combine);

        MeshFilter mf = combined.AddComponent<MeshFilter>();
        MeshRenderer mr = combined.AddComponent<MeshRenderer>();

        mf.sharedMesh = combinedMesh;

        // 첫 번째 블록의 머티리얼을 그대로 사용
        MeshRenderer firstRenderer = meshFilters[0].GetComponent<MeshRenderer>();
        if (firstRenderer != null)
            mr.sharedMaterial = firstRenderer.sharedMaterial;

        // ? 기존 블록 모두 안전하게 삭제
        foreach (GameObject block in originalBlocks)
        {
            if (block != null)
                DestroyImmediate(block);
        }

        Debug.Log($"? {meshFilters.Count}개의 블록이 하나의 메쉬로 병합되었습니다!");
    }

    /// <summary>
    /// 이미 존재하는 블록들을 모두 삭제합니다.
    /// </summary>
    public void ClearGround()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        foreach (GameObject child in children)
        {
            if (Application.isEditor)
                DestroyImmediate(child);
            else
                Destroy(child);
        }
    }

    /// <summary>
    /// 에디터용 호출 함수
    /// </summary>
    public void GenerateGroundInEditor()
    {
        if (blockPrefab == null)
        {
            Debug.LogWarning("?? blockPrefab이 설정되지 않았습니다!");
            return;
        }

        GenerateGround();
    }
}
