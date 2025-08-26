영상 : www.youtube.com/watch?v=Xo9gYNK1tGE

UI Toolkit을 활용한 노드 기반 텍스트 정보 편집 에디터 툴입니다.
향후 프로젝트에 사용할 범용 Dialogue System을 목적으로 1인 개발했습니다.

> 노드별 이벤트 정보 저장 구현

- Inspector에서 UnityEvent를 설정하면 각 대사에 맞는 이벤트를 간편하게 호출할 수 있게 함 
- 변동성이 높은 이벤트 정보는 ScriptableObject가 아닌 씬에 종속되게 하여 결합도를 낮춤
- 관리자 객체가 대사 정보, 인물 정보 등을 관리하게 하여 사용 시 이해하기 쉽게 설계
