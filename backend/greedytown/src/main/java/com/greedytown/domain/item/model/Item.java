package com.greedytown.domain.item.model;

import lombok.Getter;
import lombok.Setter;

import javax.persistence.*;

@Entity
@Setter
@Getter
public class Item {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long itemSeq;

    private String itemName; // 지은 이름


    private Integer itemPrice;

    private String itemImage; // 유니티 네임

    @OneToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="ahichievementsSeq")
    private Achievements achievementsSeq;

    @ManyToOne
    @JoinColumn(name="itemTypeSeq")
    private ItemType itemTypeSeq;

    @ManyToOne
    @JoinColumn(name="itemColorSeq")
    private ItemColor itemColorSeq;


}
