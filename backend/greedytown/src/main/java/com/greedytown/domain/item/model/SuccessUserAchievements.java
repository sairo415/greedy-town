package com.greedytown.domain.item.model;

import com.greedytown.domain.user.model.User;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import javax.persistence.*;

@Entity
@Setter
@Getter
@IdClass(SuccessUserAchievementsPK.class)
@AllArgsConstructor
@NoArgsConstructor
public class SuccessUserAchievements {

    @Id
    @ManyToOne
    @JoinColumn(name="achievementsSeq")
    private Achievements achievementsSeq;

    @Id
    @ManyToOne
    @JoinColumn(name="userSeq")
    private User userSeq;





}
