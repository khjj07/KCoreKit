# KCoreKit

개인용 Unity 공용 코어 라이브러리(Default Library Set)입니다. 여러 프로젝트에서 반복적으로 필요한 스탯/어빌리티, 세이브, 로컬라이제이션, 트윈, UI 위젯 등의 기반 시스템을 모아 어셈블리로 묶어뒀습니다.

**개발 기간**: 2026.01 ~ (지속 유지보수 중)

## 환경

- Unity 엔진 전용 라이브러리 (Assembly Definition 기반: `KCoreKit.asmdef`)
- 의존성: Addressables, DOTween(Demigiant 폴더에 포함), BroAudio, TextMesh Pro

## 설치

Unity 프로젝트의 `Assets` 또는 `Packages` 폴더 하위에 이 저장소를 서브모듈 또는 클론으로 추가하세요.

```
git submodule add https://github.com/khjj07/KCoreKit.git Assets/KCoreKit
```

이후 Package Manager에서 Addressables, TextMesh Pro가 설치되어 있는지 확인하면 됩니다.

## 사용법

각 시스템은 독립된 폴더 + 네임스페이스로 구성되어 있어 필요한 모듈만 골라 참조할 수 있습니다.

```csharp
using KCoreKit.Stat;
using KCoreKit.Tween;

// 스탯 시스템
var stat = new StatContainer();
stat.AddModifier(StatType.Attack, 10);

// DOTween 래퍼
this.transform.DOMoveTween(targetPos, 0.3f);
```

## 프로젝트 구조

```
Scripts/
├── Ability / Stat / Attribute       # 스탯·어빌리티 시스템
├── DataTable                         # 게임 데이터 테이블 관리
├── Director / System                 # 게임 흐름/시스템 관리
├── Editor                             # 에디터 확장 툴
├── Localization                       # 다국어 지원
├── Manager                            # 각종 매니저(사운드/스프라이트 등)
├── Tween                              # DOTween 래퍼
├── Widget / Tooltip                   # UI 위젯/툴팁
├── GPUInstancing / Gizmos             # 렌더링 유틸
└── Extensions / Common / Interface    # 공용 확장 메서드·인터페이스

BroAudio/   # 오디오 라이브러리(BroAudio) 통합
Demigiant/  # DOTween 벤더링
TextMesh Pro/
Shader/
```
