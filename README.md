<img width="2237" height="1324" alt="Image" src="https://github.com/user-attachments/assets/8640fc9f-b780-4dfb-9d71-14e42f240a20" />

<br>
<br>

영상 : www.youtube.com/watch?v=Xo9gYNK1tGE

<br>

>- 유형 : 에디터 툴
>- 인원 : 1인
>- 엔진 : Unity
>- 플랫폼 : PC

### 요약

- 범용 대화 시스템 애셋을 만들기 위한 목적으로 개발
- 마우스 클릭과 드래그로 사용자 친화적인 노드 선택 및 편집
- 노드에 할당했던 UnityEvent를 통해 이벤트 발생시킴

### 1. 노드 기반 대화 편집기를 만든 이유

- 기존 솔루션들은 과하게 방대하여 배우기 어렵거나, 특정 목적만을 위해 만들어져 유연하지 못함
- 사용과 학습이 쉽고, 코드를 직접 수정하여 기능을 추가할 수 있는 작고 간단한 툴을 목표함
- 최종 목표는 애셋 스토어에 무료로 배포하는 것 (다른 프로젝트들을 진행하느라 보류 중)
  - 엑셀같은 외부 편집기가 아니라 유니티에서의 사용만 고려함
  - 선형적이지 않은 다양한 선택지 분기가 있다면, 어차피 시각화 없이 맥락 파악 불가능하기 때문
  - 외부 번역이 필요하다면 Importer/Exporter를 구현하면 될 거라고 생각


### 2. 노드 데이터 관리

- 노드들은 TextNodeData 자료형을 통해 List에 저장함
  - 편집기에서 저장할 때마다 새로 List를 만들어 저장하기 때문에, 메모리 단편화 일어나지 않음
- 노드 데이터를 불러올 땐 갖고 있던 guid 키값을 기반으로 Dictionary에 적재하여 O(1)만에 접근

### 3. 특정 노드 진입 시 실행시킬 메서드 정보 저장
- Reflection을 쓰기 위해 ScriptableObject에 씬 오브젝트를 참조하려고 했으나, Unity에서 막음.
- Timeline Package에서는 씬 오브젝트 참조가 가능한 것을 떠올림. ExposedReference API를 이용하고 있다는 것을 알아냄. 하지만 씬 객체와 메서드를 직접 참조하니 변화에 취약해짐.
- 결합도를 낮추기 위해 string 별칭으로 UnityEvent를 추상화하여 참조
- ScriptableObject에 연결할 UnityEvent를 관리해야 했음. Narrator 객체가 Actor 객체들을 관리하게 하고, Actor에는 string 별칭과 함께 UnityEvent를 저장함. 
- 편집창에서 Narrator만 가져오면, Text Tree SO에 TT Actor의 UnityEvent를 저장 가능.
