# KCoreKit

Reusable Unity foundation library, consumed by host projects as a git submodule
(`https://github.com/khjj07/KCoreKit`). It is **not** game code — nothing here may
depend on a specific game's rules, content, or data.

Two standing rules govern how Claude works with this library:

1. Reuse what is already here instead of writing it again (below).
2. Propose promoting genuinely reusable code *into* here — but only after
   verifying collaborator access (below).

---

## Rule 1 — Reach for KCoreKit before writing new code

**Before writing any gameplay-adjacent utility, check whether KCoreKit already
provides it.** Search this folder first; only write something new when nothing
here fits.

| Folder | Provides |
|---|---|
| `Ability/` | `AbilityAgent`, `AbilityProvider`, `AbilityEffect`, `IAbilityContext` — data-driven ability pipeline |
| `Animation/` | `AnimationCallbackBehaviour` — animation event → C# callback |
| `Attribute/` | Inspector attributes + drawers: `ButtonAttribute`, `ShowIfAttribute`, `ReadOnlyAttribute`, `RelativeRangeAttribute`, `FilterObjectPickerAttribute`, `BigHeaderAttribute`, `ShowChildrenAttribute` |
| `Build/` | `TeamcityBuildHelper`, `TeamcityBuildSetting` — CI build entry points |
| `Common/` | `Singleton<T>`, `SingletonAsset<T>`, `PrefabPool<T>`, `BigNumber`, `TransformData`, `DontDestroyAndDistinct`, `SpriteOutliner`, `SpriteRenderGroup` |
| `DataTable/` | `DataTable`, `DataTableRowBase`, `CSVReader`, `CSVRemoteManager` — CSV-backed data tables |
| `Director/` | `DirectorBase`, `DirectorFacade`, `IDirector` — ordered async initialization + service lookup |
| `Extensions/` | `CollectionExtension`, `StringExtension`, `TypeExtension`, `AddressableExtension`, `GizmosExtension`, `DebugExtension` |
| `GPUInstancing/` | `GPUInstancingManager`, `InstanceBase`, `InstancingGroupBase` |
| `Gizmos/` | `GizmosCubeDrawer`, `GizmosSphereDrawer`, `GizmosMeshDrawer`, `GizmosLabelDrawer` |
| `Interface/` | `IView<TModel>`, `ISerializeData`, `IRequest`, `RequestBase` |
| `Localization/` | `LocalizedTextComponent`, `LocalizedImageComponent`, `LocalizedTextPrinterComponent`, `LocalizedDataTableRowBase<T>` |
| `Manager/` | `DataTableManager`, `LocalizationManager`, `PrefabManager`, `InputManager`, `LoadingManager`, `PrinterManager` |
| `Printer/` | `Printer`, `Letter`, `PrintStyle` — per-character animated text |
| `Stat/` | `Stat`, `StatAgent`, `StatModifier`, `StatModifyType` |
| `System/` | `SaveSystem`, `RandomSystem`, `AbilitySystem` |
| `Tooltip/` | `TooltipWidget`, `TooltipProvider`, `TooltipDirector`, `TooltipContext` |
| `Tween/` | `TweenAnimationPlayer`, `TransformTweenData`, `TweenCombineMode` |
| `Widget/` | `WidgetBase` and subclasses: `ButtonWidget`, `TextWidget`, `ImageWidget`, `GaugeWidget`, `SliderWidget`, `PanelWidget`, `PanelModelWidget<TModel>`, `TabMenuWidget`, `WidgetUtility` |

### Common entry points

```csharp
DirectorFacade.GetDirector<T>()                  // 디렉터(서비스) 조회
DirectorFacade.WaitUntilInitialized()            // 초기화 완료 대기 (IEnumerator)

PrefabManager.Create<T>(name)                    // 프리팹 인스턴스 생성
PrefabManager.CachePrefab<T>(name)               // 프리팹 캐싱

DataTableManager.FindRow<T>(id)                  // 데이터 테이블 행 조회
DataTableManager.FindRow<T>(predicate)
DataTableManager.FindRowsByTag<T>(tag)

LocalizationManager.GetLocalizedText(key)        // 로컬라이즈 문자열
LocalizationManager.GetFontAsset(index)          // 언어별 폰트
LocalizationManager.GetLocalizedSprite(key)

SaveSystem.Save<T>(data, fileName, directory)    // T : ISerializeData
SaveSystem.Load<T>(fileName, directory, out data)
```

### Concretely, prefer these over hand-rolling

- **`WidgetBase` 는 Canvas 하위 UI 오브젝트에만 붙입니다.** UI 컴포넌트라면
  `MonoBehaviour` 직접 상속 대신 `WidgetBase` 를 상속하세요 — `canvas`, `canvasGroup`,
  `rectTransform`, 포인터 이벤트, `Show()`/`Hide()` 가 이미 있습니다.

  UI 가 아닌 것에는 **붙이지 마세요.** 월드 오브젝트, 매니저, 순수 로직 컴포넌트는
  일반 `MonoBehaviour` 입니다. `WidgetBase` 에는
  `[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]` 가 걸려 있어서
  붙이는 순간 `Transform` 이 `RectTransform` 으로 바뀌고 쓰지도 않는 `CanvasGroup`
  이 따라붙습니다. 포인터 이벤트도 Canvas / GraphicRaycaster 아래가 아니면 아예
  동작하지 않습니다.
- 싱글턴은 **`Singleton<T>`** / **`SingletonAsset<T>`** 사용. 직접 구현 금지.
- 사용자에게 보이는 문자열은 **반드시** `LocalizationManager.GetLocalizedText(key)`.
  하드코딩된 표시 문자열은 리뷰에서 거부 대상입니다.
- 밸런스 수치·콘텐츠 정의는 코드 상수가 아니라 **`DataTable`** 행으로.
- 서비스 접근은 `FindObjectOfType` 대신 **`DirectorFacade.GetDirector<T>()`**.
- 프리팹 인스턴스화는 `Instantiate` 직접 호출보다 **`PrefabManager`**.
- 사운드는 BroAudio 파사드를 통해서 — `BroAudio/CLAUDE.md` 참고.

기존 기능이 요구사항에 **부분적으로만** 맞으면, 복사해서 변형하지 말고 먼저
KCoreKit 쪽을 확장할 수 있는지 검토하고 그 판단을 사용자에게 알리세요.

---

## Rule 2 — Proposing promotion into KCoreKit

When Claude writes code in a **host project** (e.g. `Assets/Carlot/`) that turns out
to be library-shaped, it should offer to move it into KCoreKit.

### Preconditions — all must hold

**(a) The developer must be a collaborator on `khjj07/KCoreKit`.**

KCoreKit is a separate repository. A developer without write access cannot land the
change, so proposing it only wastes their time. Verify before proposing:

```bash
gh api "repos/khjj07/KCoreKit/collaborators/$(gh api user --jq .login)" --silent
```

Exit 0 (HTTP 204) → collaborator. Non-zero (404) → not a collaborator.

> **Current state: `gh` is not installed on this machine, so this check cannot be
> automated.** Until it is available, **do not propose promotion on your own** —
> either ask the developer to confirm their collaborator status explicitly, or skip
> the proposal entirely. Never assume access.

**(b) The code must actually be reusable.** All of the following:

- No dependency on the host game's rules, content, entities, or scene structure
- No hardcoded game values — no magic numbers, no game-specific strings, no
  content ids, no scene or prefab names baked in
- Configurable through parameters, generics, or interfaces rather than edits
- Useful in a *different* project, not merely "not obviously game-specific"

**(c) It must not duplicate something KCoreKit already has** (see Rule 1).

If any precondition fails, leave the code in the host project and say nothing about
promotion.

### The proposal itself

Promotion is **always the developer's decision** — approve or reject. Never move code
into KCoreKit unprompted; nothing under `Assets/KCoreKit/` may be created or modified
as a side effect of host-project work.

When proposing, state plainly:

1. What the code does, and which KCoreKit folder it would live in
2. Why it is project-independent — address (b) point by point
3. What the host project would call afterwards
4. What has to change to generalize it (renames, extracted interfaces, removed
   assumptions), and any behavior risk that carries

Then stop and wait. On approval, move it, update host call sites, and confirm both
sides still compile. On rejection, leave it where it is and do not raise it again for
that code.

### Submodule mechanics

`Assets/KCoreKit` is a **git submodule with its own history**. Changes here do not
belong to the host project's commits.

- Commit inside `Assets/KCoreKit/` first, then commit the updated gitlink in the host repo
- Never mix library and game changes in one commit
- Say explicitly when a change lands in the submodule, so the developer knows a second
  push is required

---

## Library constraints

- **Dependencies**: Addressables, DOTween (see `README.md`). Do not add a new
  third-party dependency without asking — it propagates to every consuming project.
- **Assembly layout**: `Scripts/KCoreKit.asmdef` is an **Any Platform** assembly.
  - Editor-only code inside it must be wrapped in `#if UNITY_EDITOR`, or the player
    build breaks. This is the established convention here.
  - An Any Platform assembly **cannot** reference an Editor-only assembly — Unity
    drops such references silently, with no error. Code needing types from an
    Editor-only assembly must live in its own Editor-only asmdef
    (see `Scripts/Editor/BroAudio/` for the pattern).
- **Vendored third-party code** — `Demigiant/` (DOTween), `ANU/` (debug console),
  `BroAudio/`, `TextMesh Pro/` — is upstream code. Do not edit it to fix a host-project
  problem; changes are lost on update. Flag the issue and fix it on the KCoreKit or
  host side instead.
