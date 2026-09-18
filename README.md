# KCoreKit

개인용 Unity 공용 코어 라이브러리(Default Library Set)입니다. 여러 프로젝트에서 반복적으로 필요한 스탯/어빌리티, 세이브, 로컬라이제이션, 트윈, UI 위젯 등의 기반 시스템을 모아 어셈블리로 묶어뒀습니다.

**개발 기간**: 2026.01 ~ (지속 유지보수 중)

## 환경

- Unity 엔진 전용 라이브러리 (Assembly Definition 기반: `KCoreKit.asmdef`)
- 의존성: Addressables, DOTween(호스트 프로젝트가 직접 설치, 아래 참고), BroAudio, TextMesh Pro

## 설치

Unity 프로젝트의 `Assets` 또는 `Packages` 폴더 하위에 이 저장소를 서브모듈 또는 클론으로 추가하세요.

```
git submodule add https://github.com/khjj07/KCoreKit.git Assets/KCoreKit
```

이후 Package Manager에서 Addressables, TextMesh Pro가 설치되어 있는지 확인하면 됩니다.

### DOTween

KCoreKit은 DOTween 바이너리를 벤더링하지 않습니다 — 서브모듈 안에 DOTween 사본을 들고 있으면,
호스트 프로젝트가 이미 자체적으로 DOTween을 설치한 경우 같은 이름의 어셈블리가 두 곳에 존재하게
되어 충돌하거나, `KCoreKit.asmdef`가 참조하던 GUID(벤더링된 사본에 고정된 값)와 호스트 프로젝트의
실제 DOTween 사본의 GUID가 달라서 참조가 깨지는 문제가 있었습니다.

DOTween은 Unity Package Manager(UPM)로 배포되지 않습니다(OpenUPM에도 공식 패키지가 없습니다).
호스트 프로젝트가 아래 두 경로 중 하나로 **DOTween을 직접, 딱 한 카피만** 설치하세요.

- Unity Asset Store의 무료 에셋 "DOTween (HOTween v2)" 설치, 또는
- [공식 사이트](https://dotween.demigiant.com/download.php)에서 zip 다운로드 후 `Assets/` 하위에 임포트

설치 후 Unity 메뉴 `Tools > Demigiant > DOTween Utility Panel`에서 `Setup DOTween...`을 한 번
실행해 모듈을 활성화하세요.

`KCoreKit.asmdef`는 `DOTween.Modules` 어셈블리를 GUID가 아니라 **이름**으로 참조합니다. 프로젝트에
DOTween 사본이 정확히 하나만 존재하면(즉 서브모듈 쪽에서 별도로 벤더링하지 않으면) 그 사본의 GUID가
무엇이든 이름만 일치하면 참조가 정상적으로 해석됩니다. KCoreKit 코드는 core DOTween(`DG.Tweening`)만
사용하며 DOTweenPro는 필요하지 않습니다.

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
TextMesh Pro/
Shader/
```
