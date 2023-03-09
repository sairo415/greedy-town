package com.greedytown.domain.item.model;


import lombok.Data;


import java.io.Serializable;

@Data
public class ItemUserListPK  implements Serializable {

    private Long userIndex;
    private Long itemIndex;
}
