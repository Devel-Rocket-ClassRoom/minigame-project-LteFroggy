// Firebase 초기화 진행 상태입니다.
public enum InitState {
	// 초기화가 아직 끝나지 않은 상태입니다.
	Pending,
	// 초기화가 성공해서 Firebase 기능을 사용할 수 있는 상태입니다.
	Ready,
	// 초기화에 실패해 Firebase 기능을 사용할 수 없는 상태입니다.
	Failed,
	// Firebase 기능을 사용하지 않아 초기화를 시도하지 않은 상태입니다. 
	Disabled,
}