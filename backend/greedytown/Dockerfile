FROM openjdk:11-jdk
ARG JAR_FILE=./build/libs/*.jar
COPY ${JAR_FILE} app.jar
CMD chmod +x ./gradlew && ./gradlew bootJar && java -jar /app.jar
# ENTRYPOINT ["java","-jar","/app.jar"]
