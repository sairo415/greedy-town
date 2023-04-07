# 포팅 매뉴얼

# greedy town 포팅 매뉴얼

## 목차

1. 환경 설정
2. 실행 순서
3. 주의 사항
4. 기술 스택
5. 프로젝트 구조
6. 부가설명

## 1. 환경 설정

- 하드웨어 정보
    - Architecture: x86_64
    - CPU Model: 11th Gen Intel(R) Core(TM) i7-11600H @ 2.90GHz 2.92 GHz
    - CPU family: 6
    - CPU op-mode(s): 32-bit, 64-bit
    - Socket(s): 1
    - Core(s) per socket: 4
    - Thread(s) per core: 1
    - CPU(s): 4
    - CPU MHz: 2300.167
- 소프트웨어 정보
    - OS : Ubuntu 20.04 LTS focal
    - 배포에 필요한 프로그램 : Docker version 20.10.12, build 20.10.12-0ubuntu2~20.04.1
    - 패키지 관리자 : apt 2.0.2ubuntu0.2 (amd64)

## 2. 실행 순서

### 1.docker, docker-compose 설치

도커를 사용하여 서비스를 배포하기 때문에 프레임워크, 라이브러리 등 추가 설치 할 필요 없습니다.

여러 도커 컨테이너를 관리하는데 편리한 docker-compose를 사용해서 프로젝트를 배포합니다.

1. 리눅스 패키지 업데이트
    
    ```bash
    $ sudo apt update
    ```
    
2. Docker 설치를 위한 패키지 설치
    
    ```bash
    $ sudo apt install apt-transport-https ca-certificates curl gnupg lsb-release
    ```
    
3. Docker 공식 GPG키 추가
    
    ```bash
    $ curl -fsSL <https://download.docker.com/linux/ubuntu/gpg> | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg
    ```
    
4. Docker 저장소 추가
    
    ```bash
    $ echo "deb [arch=amd64 signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] <https://download.docker.com/linux/ubuntu> $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
    ```
    
5. Docker 설치
    
    ```bash
    $ sudo apt update
    $ sudo apt install docker-ce docker-ce-cli containerd.io
    ```
    
6. Docker 서비스 실행
    
    ```bash
    $ sudo systemctl start docker
    $ sudo systemctl enable docker
    ```
    
7. Docker 버전 확인
    
    ```bash
    $ docker --version
    ```
    
8. Docker Compose 설치
    
    ```bash
    $ sudo curl -L "<https://github.com/docker/compose/releases/download/1.29.2/docker-compose-$>(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    $ sudo chmod +x /usr/local/bin/docker-compose
    $ docker-compose --version
    ```
    

### 2. 프로젝트.env 환경변수 값 수정

각 프로젝트에 ~.env 파일들이 존재합니다. docker-compose.yml파일에서 주입할 환경 변수 값들을 수정합니다.
docker-compose config 명령어를 사용해서 기입한 환경 변수들에 대한 docker-compose 환경을 확인할 수 있습니다.

### 3. docker-compose 실행

다음 명령어를 입력합니다.

```bash
$ docker-compose up #-d 옵션을 사용하면 로그출력없이 bg에서 실행
```

프로젝트를 중지하고자 한다면 다음 명령어를 수행합니다.

```bash
$ docker-compose stop # stop대신 down을 사용하면 컨테이너가 중지되고 삭제
```

### 4. 포톤 클라우드 APP ID 생성

포톤 사이트([https://www.photonengine.com/ko-kr](https://www.photonengine.com/ko-kr))에 로그인 후 create new app을 클릭 후 multiplayer game을 선택하여 create합니다. 생성된 app id를 유니티의 window>photon unity networking>PUN Wizard에 들어가 setup project에 입력합니다.

## 3. 주의 사항

1. docker, docker-compose는 아키텍처, 운영체제 등에 따라 설치 코드가 다를 수 있으니 적절한 버전을 선택하세요.
2. 기본적으로 도커를 실행하려면 root계정 권한이 필요합니다. 일반 계정에서 도커를 사용하고자 한다면 도커를 사용할 수 있는 그룹에 사용자를 추가해야 합니다. 그룹에 사용자를 추가하고자 한다면 다음 명령어를 입력합니다.

```bash
$ sudo usermod -aG docker 유저계정
```

1. 도커 설정 등에 의해서 Permission definded 에러가 나온다면 다음 명령어를 프로젝트에 적용하세요.

```bash
$ chown -R 사용자계정 프로젝트root폴더
```

## 4. 기술 스택 & 개발 환경

**Backend - Java, Spring, Photon**

- IntelliJ IDE 2022.3.1
- SpringBoot Gradle 2.7.9
- Spring Data JPA
- Spring Security
- JWT
- Lombok
- Redis 3.0.504
- MySQL 8.0.31
- Swagger 3.0.0
- Photon

**Frontend - Unity, C#**

- Unity Hub 3.4.1
- Unity 2021.3.9f1 LTS

**CI/CD**

- AWS EC2
- Docker

## 5. 프로젝트 구조

유니티는 에셋이 많아 폴더 구조가 안 뽑히는 관계로 백엔드 디렉토리 구조만 올립니다.

유니티 부분은 zip파일을 압축해제 해서 사용합니다.

```
backend
 ┣ greedytown
 ┃ ┣ .gradle
 ┃ ┃ ┣ 7.6.1
 ┃ ┃ ┃ ┣ checksums
 ┃ ┃ ┃ ┃ ┣ checksums.lock
 ┃ ┃ ┃ ┃ ┣ md5-checksums.bin
 ┃ ┃ ┃ ┃ ┗ sha1-checksums.bin
 ┃ ┃ ┃ ┣ dependencies-accessors
 ┃ ┃ ┃ ┃ ┣ dependencies-accessors.lock
 ┃ ┃ ┃ ┃ ┗ gc.properties
 ┃ ┃ ┃ ┣ executionHistory
 ┃ ┃ ┃ ┃ ┣ executionHistory.bin
 ┃ ┃ ┃ ┃ ┗ executionHistory.lock
 ┃ ┃ ┃ ┣ fileChanges
 ┃ ┃ ┃ ┃ ┗ last-build.bin
 ┃ ┃ ┃ ┣ fileHashes
 ┃ ┃ ┃ ┃ ┣ fileHashes.bin
 ┃ ┃ ┃ ┃ ┣ fileHashes.lock
 ┃ ┃ ┃ ┃ ┗ resourceHashesCache.bin
 ┃ ┃ ┃ ┣ vcsMetadata
 ┃ ┃ ┃ ┗ gc.properties
 ┃ ┃ ┣ buildOutputCleanup
 ┃ ┃ ┃ ┣ buildOutputCleanup.lock
 ┃ ┃ ┃ ┣ cache.properties
 ┃ ┃ ┃ ┗ outputFiles.bin
 ┃ ┃ ┣ vcs-1
 ┃ ┃ ┃ ┗ gc.properties
 ┃ ┃ ┗ file-system.probe
 ┃ ┣ .idea
 ┃ ┃ ┣ modules
 ┃ ┃ ┃ ┣ com.greedytown.main.iml
 ┃ ┃ ┃ ┗ greedytown.main.iml
 ┃ ┃ ┣ .gitignore
 ┃ ┃ ┣ compiler.xml
 ┃ ┃ ┣ gradle.xml
 ┃ ┃ ┣ jarRepositories.xml
 ┃ ┃ ┣ misc.xml
 ┃ ┃ ┣ modules.xml
 ┃ ┃ ┣ vcs.xml
 ┃ ┃ ┗ workspace.xml
 ┃ ┣ build
 ┃ ┃ ┣ classes
 ┃ ┃ ┃ ┗ java
 ┃ ┃ ┃ ┃ ┣ main
 ┃ ┃ ┃ ┃ ┃ ┗ com
 ┃ ┃ ┃ ┃ ┃ ┃ ┗ greedytown
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ domain
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ item
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ controller
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ ItemController.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ dto
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ AchievementsDto$AchievementsDtoBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ AchievementsDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ BuyItemDto$BuyItemDtoBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ BuyItemDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ BuyItemReturnDto$BuyItemReturnDtoBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ BuyItemReturnDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemDto$ItemDtoBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ WearingDto$WearingDtoBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ WearingDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ model
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ Achievements.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ Item.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemColor.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemType.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemUserList.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemUserListPK.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MoneyLog$MoneyLogBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MoneyLog.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ SuccessUserAchievements.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ SuccessUserAchievementsPK.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ Wearing.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ repository
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ AchievementsRepository.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemRepository.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemUserListRepository.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ SuccessUserAchievementsRepository.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ WearingRepository.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ service
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemService.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ ItemServiceImpl.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ social
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ controller
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ SocialController.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ dto
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ FriendUserListDto$FriendUserListDtoBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ FriendUserListDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MessageDto$MessageDtoBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MessageDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MyFriendDto$MyFriendDtoBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MyFriendDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MyMessageDto$MyMessageDtoBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MyMessageDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ RankingDto$RankingDtoBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ RankingDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ model
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ FriendUserList.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ Message.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ Stat.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ repository
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ FriendUserListRepository.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MessageRepository.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MoneyLogRepository.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ StatRepository.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ service
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ SocialService.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ SocialServiceImpl.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ user
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ controller
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ Logincontroller.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ UserController.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ dto
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ EarnMoneyDto$EarnMoneyDtoBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ EarnMoneyDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ LoginRequestDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ StatDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ TokenDto$TokenDtoBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ TokenDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ UserDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ UserInfoDto$UserInfoDtoBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ UserInfoDto.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ model
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ User$UserBuilder.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ User.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ repository
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ UserRepository.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ service
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ UserService.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ UserServiceImpl.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ global
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ config
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ auth
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ PrincipalDetails.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ PrincipalDetailsService.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ jwt
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ JwtAuthenticationFilter.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ JwtAuthorizationFilter.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ JwtProperties.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ RedisConfig.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ SecurityConfig.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ SwaggerConfig.class
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ GreedytownApplication.class
 ┃ ┃ ┃ ┃ ┗ test
 ┃ ┃ ┃ ┃ ┃ ┗ com
 ┃ ┃ ┃ ┃ ┃ ┃ ┗ greedytown
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ GreedytownApplicationTests.class
 ┃ ┃ ┣ generated
 ┃ ┃ ┃ ┗ sources
 ┃ ┃ ┃ ┃ ┣ annotationProcessor
 ┃ ┃ ┃ ┃ ┃ ┗ java
 ┃ ┃ ┃ ┃ ┃ ┃ ┣ main
 ┃ ┃ ┃ ┃ ┃ ┃ ┗ test
 ┃ ┃ ┃ ┃ ┗ headers
 ┃ ┃ ┃ ┃ ┃ ┗ java
 ┃ ┃ ┃ ┃ ┃ ┃ ┣ main
 ┃ ┃ ┃ ┃ ┃ ┃ ┗ test
 ┃ ┃ ┣ libs
 ┃ ┃ ┃ ┗ greedytown-0.0.1-SNAPSHOT.jar
 ┃ ┃ ┣ reports
 ┃ ┃ ┃ ┗ tests
 ┃ ┃ ┃ ┃ ┗ test
 ┃ ┃ ┃ ┃ ┃ ┣ classes
 ┃ ┃ ┃ ┃ ┃ ┃ ┗ com.greedytown.GreedytownApplicationTests.html
 ┃ ┃ ┃ ┃ ┃ ┣ css
 ┃ ┃ ┃ ┃ ┃ ┃ ┣ base-style.css
 ┃ ┃ ┃ ┃ ┃ ┃ ┗ style.css
 ┃ ┃ ┃ ┃ ┃ ┣ js
 ┃ ┃ ┃ ┃ ┃ ┃ ┗ report.js
 ┃ ┃ ┃ ┃ ┃ ┣ packages
 ┃ ┃ ┃ ┃ ┃ ┃ ┗ com.greedytown.html
 ┃ ┃ ┃ ┃ ┃ ┗ index.html
 ┃ ┃ ┣ resources
 ┃ ┃ ┃ ┗ main
 ┃ ┃ ┃ ┃ ┗ application.properties
 ┃ ┃ ┣ test-results
 ┃ ┃ ┃ ┗ test
 ┃ ┃ ┃ ┃ ┣ binary
 ┃ ┃ ┃ ┃ ┃ ┣ output.bin
 ┃ ┃ ┃ ┃ ┃ ┣ output.bin.idx
 ┃ ┃ ┃ ┃ ┃ ┗ results.bin
 ┃ ┃ ┃ ┃ ┗ TEST-com.greedytown.GreedytownApplicationTests.xml
 ┃ ┃ ┣ tmp
 ┃ ┃ ┃ ┣ bootJar
 ┃ ┃ ┃ ┃ ┗ MANIFEST.MF
 ┃ ┃ ┃ ┣ compileJava
 ┃ ┃ ┃ ┃ ┣ compileTransaction
 ┃ ┃ ┃ ┃ ┃ ┣ annotation-output
 ┃ ┃ ┃ ┃ ┃ ┣ compile-output
 ┃ ┃ ┃ ┃ ┃ ┃ ┗ com
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ greedytown
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ domain
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ item
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ controller
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ dto
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ model
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ repository
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ service
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ social
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ controller
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ dto
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ model
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ repository
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ service
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ user
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ controller
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ dto
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ model
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ repository
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ service
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ global
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ config
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ auth
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ jwt
 ┃ ┃ ┃ ┃ ┃ ┣ header-output
 ┃ ┃ ┃ ┃ ┃ ┗ stash-dir
 ┃ ┃ ┃ ┃ ┃ ┃ ┣ SocialServiceImpl.class.uniqueId1
 ┃ ┃ ┃ ┃ ┃ ┃ ┣ StatRepository.class.uniqueId0
 ┃ ┃ ┃ ┃ ┃ ┃ ┗ UserServiceImpl.class.uniqueId2
 ┃ ┃ ┃ ┃ ┗ previous-compilation-data.bin
 ┃ ┃ ┃ ┣ compileTestJava
 ┃ ┃ ┃ ┃ ┗ previous-compilation-data.bin
 ┃ ┃ ┃ ┣ jar
 ┃ ┃ ┃ ┃ ┗ MANIFEST.MF
 ┃ ┃ ┃ ┗ test
 ┃ ┃ ┗ bootJarMainClassName
 ┃ ┣ gradle
 ┃ ┃ ┗ wrapper
 ┃ ┃ ┃ ┣ gradle-wrapper.jar
 ┃ ┃ ┃ ┗ gradle-wrapper.properties
 ┃ ┣ src
 ┃ ┃ ┣ main
 ┃ ┃ ┃ ┣ java
 ┃ ┃ ┃ ┃ ┗ com
 ┃ ┃ ┃ ┃ ┃ ┗ greedytown
 ┃ ┃ ┃ ┃ ┃ ┃ ┣ domain
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ item
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ controller
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ ItemController.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ dto
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ AchievementsDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ BuyItemDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ BuyItemReturnDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ WearingDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ model
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ Achievements.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ Item.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemColor.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemType.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemUserList.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemUserListPK.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MoneyLog.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ SuccessUserAchievements.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ SuccessUserAchievementsPK.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ Wearing.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ repository
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ AchievementsRepository.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemRepository.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemUserListRepository.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ SuccessUserAchievementsRepository.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ WearingRepository.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ service
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ ItemService.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ ItemServiceImpl.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ social
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ controller
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ SocialController.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ dto
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ FriendUserListDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MessageDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MyFriendDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MyMessageDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ RankingDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ model
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ FriendUserList.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ Message.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ Stat.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ repository
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ FriendUserListRepository.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MessageRepository.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ MoneyLogRepository.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ StatRepository.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ service
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ SocialService.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ SocialServiceImpl.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ user
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ controller
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ Logincontroller.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ UserController.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ dto
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ EarnMoneyDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ LoginRequestDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ StatDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ TokenDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ UserDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ UserInfoDto.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ model
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ User.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ repository
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ UserRepository.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ service
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ UserService.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ UserServiceImpl.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┣ global
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ config
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ auth
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ PrincipalDetails.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ PrincipalDetailsService.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ jwt
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ JwtAuthenticationFilter.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ JwtAuthorizationFilter.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ JwtProperties.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ RedisConfig.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┣ SecurityConfig.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┃ ┗ SwaggerConfig.java
 ┃ ┃ ┃ ┃ ┃ ┃ ┗ GreedytownApplication.java
 ┃ ┃ ┃ ┗ resources
 ┃ ┃ ┃ ┃ ┗ application.properties
 ┃ ┃ ┗ test
 ┃ ┃ ┃ ┗ java
 ┃ ┃ ┃ ┃ ┗ com
 ┃ ┃ ┃ ┃ ┃ ┗ greedytown
 ┃ ┃ ┃ ┃ ┃ ┃ ┗ GreedytownApplicationTests.java
 ┃ ┣ .gitignore
 ┃ ┣ build.gradle
 ┃ ┣ Dockerfile
 ┃ ┣ gradlew
 ┃ ┣ gradlew.bat
 ┃ ┗ settings.gradle
 ┣ input
 ┃ ┗ greedytown_sql.png
 ┣ backend.env
 ┗ docker-compose.yml
├─db
│  ├─mysql
│  │  └─res
│  │      └─data
│  └─redis
│      └─res
│          └─data
```

## 6. 부가 설명

1. Mysql 더미파일 : greedytown_dump.sql

2. 시나리오
- 회원가입 후 로그인. 이후 개인 옷장에서 시작
- 착용할 아이템 착용하고 로비로 나감 ( 오른쪽 위 홈버튼)
그 오른쪽은 로그아웃 버튼
- 맵 중앙에 소환 뒤 쪽의 큰 보라색 건물은 상점 진입 후 구매
    원하는 아이템 구매 후 로비로 복귀
- 왼쪽으로 쭉 가서 보스 레이드 입장 방 만들고 위의 포탈로 접근하여 게임 시작(여러명일 경우 기다려서 함께 포탈로 진입)
- 보스레이드 플레이 이후 12시방향의 서바이벌 진입 플레이. 
- 이후 3시 방향의 카지노 진입 세 개임 모두 플레이
- 이후 5시 방향의 경찰서 진입 pvp플레이
