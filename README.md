## 루프
<p align="center">
	<img src="./images/Loop.png">
</p>

매니저
>싱글톤 패턴의 플레이어, 소리, 카드, 장비, 스펙 등등의 매니저를 사용.


<br>이벤트
>대리자(Action)를 사용. 옵저버 패턴으로 전투 승리, 패배, 레벨 업 등의 이벤트를 관찰하고 적절한 로직을 수행토록 함.

<br>데이터 저장/불러오기, 로그인.
>Unity Cloud를 사용.<br>
><a href="https://github.com/HomebodyDevil/INUniversity/blob/master/Capstone/Assets/Scripts/Data/CloudData.cs">CloudData.cs(로그인)</a><br>
><a href="https://github.com/HomebodyDevil/INUniversity/blob/master/Capstone/Assets/Scripts/Data/DataManager.cs">DataManager.cs(클라우드 저장/불러오기)</a>

<br><br><br>



## GPS
<p align="center">
	<img src="./images/Loop.png">
</p>

사용자가 중점으로부터 일정 거리 안(Area)에 들어왔다면, 사용자의 위치를 게임 상에 적용시킨다.
지정된 해당 Area의 기준점으로부터 가로(a), 세로(b)의 거리를 하버사인 공식을 사용하여 구한다.

Area의 기준점은 게임 상에서 (0, 0)이도록 했기 때문에, a를 x축에 대한 거리, b를 y축에 대한 거리로 적용한다.
