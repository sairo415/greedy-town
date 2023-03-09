package com.greedytown.domain.social.model;


import lombok.Data;

import java.io.Serializable;

@Data
public class FriendUserListPK implements Serializable {

    private Long userIndexA;
    private Long userIndexB;
}
