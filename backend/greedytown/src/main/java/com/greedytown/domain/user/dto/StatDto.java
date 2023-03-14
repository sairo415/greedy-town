package com.greedytown.domain.user.dto;

import com.fasterxml.jackson.annotation.JsonFormat;
import lombok.Data;
import org.springframework.format.annotation.DateTimeFormat;

import java.sql.Timestamp;
import java.time.LocalTime;

@Data
public class StatDto {


    @JsonFormat(shape = JsonFormat.Shape.STRING, pattern = "HH:mm:ss", timezone="Asia/Seoul")
    private Timestamp userClearTime;

}
