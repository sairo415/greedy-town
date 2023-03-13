package com.greedytown.domain.item.model;

import com.greedytown.domain.user.model.User;
import lombok.Getter;
import lombok.Setter;

import javax.persistence.*;

@Entity
@Setter
@Getter
public class Wearing {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long wearingSeq;

    @JoinColumn(name="userSeq")
    @OneToOne
    private User userSeq;

    @JoinColumn(name="itemSeq")
    @ManyToOne
    private Item itemSeq;






}
