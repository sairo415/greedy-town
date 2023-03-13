package com.greedytown.domain.user.dto;

import lombok.Data;

import java.sql.Timestamp;
import java.util.Date;

@Data
public class StatDto {

    private Long userSeq;
    private Timestamp userClearTime;

}
