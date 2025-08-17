# 🚩 Team7 Industry-Academia Corporate Project
- 동계올림픽 선수단 단장 시뮬레이션 모바일 게임
## 📌 Game References
- Sports City Tycoon
<p align="center">
  <img src="https://github.com/user-attachments/assets/435f7df9-dc27-4657-8880-a2c9527de764">
</p>

- Pocket League Story
<p align="center">
  <img src="https://github.com/user-attachments/assets/492ca98d-051e-44c4-96ec-0f6c4c9ccddc">
</p>

- Umamusume
<p align="center">
  <img src="https://github.com/user-attachments/assets/43a3d088-3ec9-4cce-b0f3-253c06cc84b9">
</p>


- 선수를 영입하고, 훈련시키고, 대회에 나가 입상을 하는 게임.
---
# 🚩 Rule-Set
프로젝트를 진행하면서 지켜야 할 규칙.

---
## 📝 Code 컨벤션
- 카멜 표기법으로 변수 선언.
- 대문자로 시작하는 단어로 클래스, 함수, 구조체 등 선언
- `_` 언더바를 쓰지 않는다(카멜표기법)
- `bool`과 같은 논리형 변수의 경우 `is`, `can`, `has` 을 붙여서 쓴다.(ex. `isMove`, `canJump`, etc...)

---
## 📦 Unity 컨벤션
- `Project Settings`, `Package Manager`처럼 전역적인 변경은 한 사람만 변경해서 `develop` 브랜치에 병합한다.
- 다른 팀원들은 변경 사항이 적용된 `develop` 브랜치를 본인이 작업중인 브랜치로 병합하여 적용한다.
- 본인의 작업공간에서 작성하는 스크립트는 네임스페이스를 설정한다. `namespace 본인이름이니셜` 로 한다.(ex. namespace JYL)
- 외부 에셋은 전부 `Imports` 폴더 안에서 관리하며 사용한다.
- `Imports` 폴더 안의 에셋들은 `Unity Package` 형태로 만들어서 구글드라이브에 업로드 후, 팀원들에게 공유한다.
- Ai로 생성한 에셋의 경우, 본인의 폴더 안에 위치해도 된다.
- 본인의 작업물은 **꼭 프리팹화** 하고, 씬에서 수정하면 프리팹에 변경사항을 `Override` 한다.

### 💡 프리팹 새로 만들기
- 외부 에셋 및 다른 팀원 작업 폴더에서 프리팹을 가져다가 본인의 작업공간에서 쓸 때는 다음 사항을 따른다.

1. 본인이 작업중인 씬에서 `Hierarchy 창` 또는 `Scene 창`에 사용하고자 하는 프리팹을 가져와서 생성한다.

2. `Hierarchy 창`에서 생성한 게임 오브젝트를 마우스 오른쪽 클릭 후, Prefab - UnPack 을 누른다.

3. 본인의 작업 폴더에 해당 게임 오브젝트를 `드래그&드롭` 하면 새로운 프리팹이 생성된다.

- 이는 남의 작업물인 프리팹을 실수로라도 변경되지 않게 하기 위함이다.

---
## 🌐 Git 컨벤션
---
### ✅ Commit & Push 룰셋
1. 본인이 작업중인 브랜치에서는 편한대로 `Commit` & `Push`를 한다.
2. 다른 구성원의 기능이 급하게 필요한 상황에서는 **본인의 브랜치로 해당 팀원의 브랜치를 병합**한다.
3. 단, 안전을 위해 병합 전에 본인의 브랜치에서 `Test` 브랜치 생성 후 병합하는 것을 추천한다.(ex. JYL_Player_MergeTest_상대브랜치 또는 날짜)
4. `Pull request` 하는 것은 **2명의 동의**가 필요하다.(작성자 본인, 다른 팀원 한 명)

---
### 📝 Commit Message 룰셋 
깃에 새로운 커밋을 작성할 때는 아래와 같이 헤더를 앞에 써준다.

| 타입 이름  | 내용                                                   |
|------------|--------------------------------------------------------|
| Feat       | 새로운 기능 구현에 대한 커밋                            |
| Fix        | 버그 수정에 대한 커밋                                  |
| Bug        | 버그 발생에 대한 커밋                                  |
| Build      | 빌드 관련 파일 수정 / 모듈 설치 또는 삭제에 대한 커밋 |
| Chore      | 잡다한 수정에 대한 커밋                          |
| Ci         | ci 관련 설정 수정에 대한 커밋                         |
| Docs       | 문서 추가/수정에 대한 커밋                                  |
| Style      | 코드 스타일 혹은 포맷 등에 관한 커밋                  |
| Refactor   | 코드 리팩토링에 대한 커밋                              |
| Test       | 테스트 구성 및 코드 수정에 대한 커밋                    |
| Perf       | 성능 개선에 대한 커밋 (코드 리팩토링)                |

- 커밋의 `제목(Summary)`는 다음 구조로 작성한다.
```
[타입(대문자로 시작)] 제목(영어 대문자, 명령형)
ex) [Feat] Add player movement controller
```

- 커밋의 `설명(Description)`은 `-`으로 시작하며, 한글로 작성한다. 직관적으로 볼 수 있게 어떤 방식으로 구현했는지 작성한다.
```
- 플레이어 이동 컨트롤러를 구현함. FSM을 쓸 수 있게 StateMachine을 참조하여, 현재 상태(State)의 Update를 수행함.
```

---
### ✅ Branch 사용 룰셋

#### 💡 정적 브랜치
- `main` : 이틀 내지 사흘에 한 번, 모든 작업이 끝나고(17시 전후) 병합이 완료된 `develop` 브랜치를 `main` 브랜치로 병합한다. 룰셋에 의해 개인이 직접 커밋&푸시, 삭제가 안된다.
- `develop` : 기능 구현 브랜치. 매일 17시 전후로 개인작업 내용을 병합한다. `Pull-Request`를 작성하여 코드리뷰 후 병합한다.

위 브랜치 들은 삭제하지 않는다

#### 🔧 문제 해결용 브랜치
- `hotfix`: 문제 발생 시 해결하기 위한 브랜치.(ex. develop_test_현재날짜)
- `test`: 테스트를 진행하기 위한 브랜치. `develop` 병합 시도 시, `Conflict`가 날 확률이 높을 경우 사용한다.(develop_test_오늘날짜)

위 브랜치들은 문제가 해결되면 삭제한다.

#### ✅ 브랜치 작업 방식
- 개개인의 브랜치를 만들어서 기능 구현을 한다.(ex. JYL_UIManager)
- 매일 17시가 되면, 유니티 에러가 나지 않는다면 코드리뷰를 진행한다.
- 코드리뷰를 진행하고, `develop`으로 `Pull-Request`를 작성한다.
- 매일 저녁에 다같이 `develop`에 병합할 때는 `[Merge] Daily merge {병합하는 브랜치} to {병합되는 곳 브랜치}` 로 작성한다.
```
예시) 
[Merge] Daily merge JYL_PlayerAttack to develop
// 내용은 자유롭게 한글로 작성
```

#### 💡 충돌 시 해결방법
1. 본인의 브랜치가 `develop`으로의 `Pull-Request`에서 `Conflict`가 날 경우
2. 먼저, 현재 본인의 브랜치의 test 브랜치를 만든다
```
JYL_Player를 develop에 병합 시도 중 Conflict 발생 시 => JYL_Player_test_오늘날짜
```
4. `Github Desktop`에서 `develop`을 **생성한 브랜치로 병합을 시도**하면서 Conflict를 해결한다.
5. 해결된 브랜치를 develop으로 병합한다.
6. `test 브랜치`는 삭제 후, 최신 버전이 된 **develop을 본인의 브랜치로 병합**한다.(JYL_Player로 develop을 병합하기)

---
## 🏢 Github Project 컨벤션
### 📌 칸반 구성

| 컬럼        | 역할                             |
|-------------|----------------------------------|
| To Do       | 해야할 일 (승인된 이슈)          |
| In Progress | 작업 중. **담당자 지정 필수**     |
| Done        | 기능 구현 완료                   |
| Review      | Pull Request 완료, 리뷰 대기     |
| Merge       | 머지 완료된 작업                 |

---
### 📝 Issue 작성
- 모든 작업은 반드시 이슈를 통해 시작한다.
- 기획팀이 작성한 이슈들을 하이퍼 링크로 연결한다.
```
1. 이슈 하이퍼링크
#이슈번호
#38, #128 ....

2. 노션 글이나 기획서 같은 외부 글 링크
[제목](게시글 하이퍼 링크)
[플레이어 기능 구현](링크)

3. 이미지, GIF, 영상
드래그&드롭으로 됨

```
- 이슈 제목은 짧고 명확하게 작성 (`[태그] 동사 + 객체` 형식 권장)
- 이슈 본문은 템플릿 기반 작성 (`Bug` / `Feature` / `Docs` 분리)
- 이슈를 작성할 때, Project 설정(`중요도`, `기간`, `분류`), `Milestone` 을 잊지 않는다.
- `Sub-Issue`일 경우, 부모 설정을 잊지 않는다. 
- 실수로라도 빼먹은 부분들은 팀장이 해결할 예정이니, 부담감은 가지지 말자.
```
이슈 작성 예시)

제목 : [Feat] 플레이어 점프 로직 추가

본문 : 
**구현하고자 하는 기능**
#이슈번호 : 플레이어 기획서
플레이어 클래스에 점프 기능을 추가하고자 합니다

**기능을 구현하는 방법**
어떠한 방법으로 기능을 구현할지 설명
Player 오브젝트의 RigidBody를 받아 AddForce(ForceMode.Impulse)로 구현

**대체 기능**
X

**추가 설명**
플레이어가 점프하는 힘이 현재 속도에 비례하도록 구현 바람.
```
