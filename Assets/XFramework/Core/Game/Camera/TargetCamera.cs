using UnityEngine;
using System;

/// <summary>
/// 镜头控制
/// </summary>
public class TargetCamera : MonoBehaviour
{
    #region 不同的镜头类
    public class BaseCameraMode
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize(Transform self) { }

        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="self">相机自身</param>
        /// <param name="target">目标单位</param>
        /// <param name="targetOffset">目标偏移量</param>
        public virtual void OnEnable(Transform self, Transform target, Vector3 targetOffset) { }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset(Transform self, Transform target, Vector3 targetOffset) { }

        /// <summary>
        /// 相机控制，相机的Update在Mono的LateUpdate中执行
        /// </summary>
        public virtual void Update(Transform self, Transform target, Vector3 targetOffset) { }

        /// <summary>
        /// 禁用
        /// </summary>
        public virtual void OnDisable(Transform self, Transform target, Vector3 targetOffset) { }
    }

    // 固定镜头
    [Serializable]
    public class CameraAttachTo : BaseCameraMode
    {
        /// <summary>
        /// 被附加的目标
        /// </summary>
        public Transform attachTarget;

        public override void Update(Transform self, Transform target, Vector3 targetOffset)
        {
            if (attachTarget != null) target = attachTarget;
            if (target == null) return;

            self.position = target.position;
            self.rotation = target.rotation;
        }
    }

    // 平滑镜头
    [Serializable]
    public class CameraSmoothFollow : BaseCameraMode
    {
        public float distance = 10.0f;
        public float height = 5.0f;
        public float viewHeightRatio = 0.5f;      // Look above the target (height * this ratio)
        [Space(5)]
        public float heightDamping = 2.0f;
        public float rotationDamping = 3.0f;
        [Space(5)]
        public bool followVelocity = true;
        public float velocityDamping = 5.0f;

        private Vector3 smoothLastPos = Vector3.zero;
        private Vector3 smoothVelocity = Vector3.zero;
        private float smoothTargetAngle = 0.0f;

        public override void Reset(Transform self, Transform target, Vector3 targetOffset)
        {
            if (target == null) return;

            smoothLastPos = target.position + targetOffset;
            smoothVelocity = target.forward * 2.0f;
            smoothTargetAngle = target.eulerAngles.y;
        }

        public override void Update(Transform self, Transform target, Vector3 targetOffset)
        {
            if (target == null) return;

            Vector3 updatedVelocity = (target.position + targetOffset - smoothLastPos) / Time.deltaTime;
            smoothLastPos = target.position + targetOffset;

            updatedVelocity.y = 0.0f;  // 保证相机高度不变

            if (updatedVelocity.magnitude > 1.0f)
            {
                smoothVelocity = Vector3.Lerp(smoothVelocity, updatedVelocity, velocityDamping * Time.deltaTime);
                smoothTargetAngle = Mathf.Atan2(smoothVelocity.x, smoothVelocity.z) * Mathf.Rad2Deg;
            }

            if (!followVelocity)
                smoothTargetAngle = target.eulerAngles.y;

            float wantedHeight = target.position.y + targetOffset.y + height;
            float currentRotationAngle = self.eulerAngles.y;
            float currentHeight = self.position.y;

            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, smoothTargetAngle, rotationDamping * Time.deltaTime);
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            self.position = target.position + targetOffset;
            self.position -= currentRotation * Vector3.forward * distance;

            Vector3 t = self.position;
            t.y = currentHeight;
            self.position = t;

            self.LookAt(target.position + targetOffset + Vector3.up * height * viewHeightRatio);
        }
    }

    // 鼠标控制方向的镜头  TODO : 鼠标横向快速移动时镜头不能连续旋转，镜头连续旋转度数不会超过360
    [Serializable]
    public class CameraMouseOrbit : BaseCameraMode
    {
        public float distance = 10.0f;
        [Space(5)]
        public float minVerticalAngle = -20.0f;
        public float maxVerticalAngle = 80.0f;
        public float horizontalSpeed = 5f;
        public float verticalSpeed = 2.5f;
        public float orbitDamping = 4.0f;
        [Space(5)]
        public float minDistance = 5.0f;
        public float maxDistance = 50.0f;
        public float distanceSpeed = 10.0f;
        public float distanceDamping = 4.0f;
        [Space(5)]
        public string horizontalAxis = "Mouse X";
        public string verticalAxis = "Mouse Y";
        public string distanceAxis = "Mouse ScrollWheel";

        private float orbitX = 0.0f;
        private float orbitY = 0.0f;
        private float orbitDistance;

        public override void Initialize(Transform self)
        {
            orbitDistance = distance;

            orbitX = self.eulerAngles.y;
            orbitY = self.eulerAngles.x;
        }

        public override void Update(Transform self, Transform target, Vector3 targetOffset)
        {
            if (target == null) return;

            // 输入影响数据并限制数据在一定范围内
            orbitX += Input.GetAxis(horizontalAxis) * horizontalSpeed;
            orbitY -= Input.GetAxis(verticalAxis) * verticalSpeed;
            distance -= Input.GetAxis(distanceAxis) * distanceSpeed;

            orbitY = Mathf.Clamp(orbitY, minVerticalAngle, maxVerticalAngle);
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            // 计算相机位置和角度
            orbitDistance = Mathf.Lerp(orbitDistance, distance, distanceDamping * Time.deltaTime);
            self.rotation = Quaternion.Slerp(self.rotation, Quaternion.Euler(orbitY, orbitX, 0), orbitDamping * Time.deltaTime);
            self.position = target.position + targetOffset + self.rotation * new Vector3(0.0f, 0.0f, -orbitDistance);
        }
    }

    // 定点跟随镜头 
    [Serializable]
    public class CameraLookAt : BaseCameraMode
    {
        public float damping = 6.0f;
        [Space(5)]
        public float minFov = 10.0f;    // 最小视角 Field of view
        public float maxFov = 60.0f;
        public float fovSpeed = 20.0f;
        public float fovDamping = 4.0f;
        public bool autoFov = false;
        public string fovAxis = "Mouse ScrollWheel";
        [Space(5)]
        public bool enableMovement = false;  // 是否激活镜头移动
        public float movementSpeed = 2.0f;
        public float movementDamping = 5.0f;
        public string forwardAxis = "Vertical";
        public string sidewaysAxis = "Horizontal";
        public string verticalAxis = "Mouse Y";

        private Camera camera;
        private Vector3 position;
        private float fov = 0.0f;
        private float savedFov = 0.0f;

        public override void Initialize(Transform self)
        {
            camera = self.GetComponentInChildren<Camera>();
        }

        public override void OnEnable(Transform self, Transform target, Vector3 targetOffset)
        {
            position = self.position;

            if (camera != null)
            {
                fov = camera.fieldOfView;
                savedFov = camera.fieldOfView;
            }
        }

        public override void Update(Transform self, Transform target, Vector3 targetOffset)
        {
            // 位置
            if (enableMovement)
            {
                float stepSize = movementSpeed * Time.deltaTime;

                position += Input.GetAxis(forwardAxis) * stepSize * new Vector3(self.forward.x, 0.0f, self.forward.z).normalized;
                position += Input.GetAxis(sidewaysAxis) * stepSize * self.right;
                position += Input.GetAxis(verticalAxis) * stepSize * self.up;
            }

            self.position = Vector3.Lerp(self.position, position, movementDamping * Time.deltaTime);

            // 角度
            if (target != null)
            {
                Quaternion lookAtRotation = Quaternion.LookRotation(target.position + targetOffset - self.position);
                self.rotation = Quaternion.Slerp(self.rotation, lookAtRotation, damping * Time.deltaTime);
            }

            // 拉近拉远
            if (camera != null)
            {
                fov -= Input.GetAxis(fovAxis) * fovSpeed;
                fov = Mathf.Clamp(fov, minFov, maxFov);
                camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, fov, fovDamping * Time.deltaTime);
            }
        }

        public override void OnDisable(Transform self, Transform target, Vector3 targetOffset)
        {
            if (camera != null)
                camera.fieldOfView = savedFov;
        }
    }

    #endregion

    public enum Mode { AttachTo, SmoothFollow, MouseOrbit, LookAt };
    public Mode mode = Mode.SmoothFollow;

    public Transform target;
    public bool followCenterOfMass = true;
    public KeyCode changeCameraKey = KeyCode.C;

    // 这里把相机直接声明是为了在面板中调节参数
    public CameraAttachTo attachTo = new CameraAttachTo();
    public CameraSmoothFollow smoothFollow = new CameraSmoothFollow();
    public CameraMouseOrbit mouseOrbit = new CameraMouseOrbit();
    public CameraLookAt lookAt = new CameraLookAt();

    private Mode prevMode;
    private BaseCameraMode[] cameraModes;

    private Transform prevTarget;
    private Rigidbody targetRigidbody;
    private Vector3 localTargetOffset;
    private Vector3 targetOffset;

    void OnEnable()
    {
        cameraModes = new BaseCameraMode[]
        {
            attachTo, smoothFollow, mouseOrbit, lookAt
        };

        foreach (BaseCameraMode cam in cameraModes)
            cam.Initialize(transform);


        OnTargetChange();
        ComputeTargetOffset();
        prevTarget = target;

        cameraModes[(int)mode].OnEnable(transform, target, targetOffset);
        cameraModes[(int)mode].Reset(transform, target, targetOffset);
        prevMode = mode;
    }

    void OnDisable()
    {
        cameraModes[(int)mode].OnDisable(transform, target, targetOffset);
    }

    void LateUpdate()
    {
        // 跟随目标是否变化
        if (target != prevTarget)
        {
            OnTargetChange();
            prevTarget = target;
        }

        ComputeTargetOffset();

        if (Input.GetKeyDown(changeCameraKey))
            NextCameraMode();

        if (mode != prevMode)
        {
            cameraModes[(int)prevMode].OnDisable(transform, target, targetOffset);
            cameraModes[(int)mode].OnEnable(transform, target, targetOffset);
            cameraModes[(int)mode].Reset(transform, target, targetOffset);
            prevMode = mode;
        }

        cameraModes[(int)mode].Update(transform, target, targetOffset);
    }

    /// <summary>
    /// 计算目标偏移
    /// </summary>
    void ComputeTargetOffset()
    {
        if (followCenterOfMass && targetRigidbody != null)
        {
            // 重心不受大小影响
            targetOffset = target.TransformDirection(localTargetOffset);
        }
        else
        {
            targetOffset = Vector3.zero;
        }
    }

    /// <summary>
    /// 切换镜头类型
    /// </summary>
    public void NextCameraMode()
    {
        if (!enabled) return;

        mode++;
        if ((int)mode >= cameraModes.Length)
            mode = (Mode)0;
    }

    /// <summary>
    /// 目标变化
    /// </summary>
    void OnTargetChange()
    {
        // 如果目标是刚体的话计算重心
        if (followCenterOfMass && target != null)
        {
            targetRigidbody = target.GetComponent<Rigidbody>();
            localTargetOffset = targetRigidbody.centerOfMass;
        }
        else
        {
            targetRigidbody = null;
        }

        cameraModes[(int)mode].Reset(transform, target, targetOffset);
    }
}