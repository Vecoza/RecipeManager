import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { RecipeListItem } from '../../models/recipe.models';
import { RecipeService } from '../../../core/services/recipe.service';

@Component({
  selector: 'app-recipe-card',
  standalone: true,
  imports: [RouterLink, MatButtonModule, MatIconModule, MatChipsModule],
  templateUrl: './recipe-card.component.html',
  styleUrl: './recipe-card.component.scss'
})
export class RecipeCardComponent {
  @Input({ required: true }) recipe!: RecipeListItem;
  @Output() deleted = new EventEmitter<void>();

  constructor(private recipeService: RecipeService) {}

  onDelete() {
    this.recipeService.delete(this.recipe.id).subscribe(() => this.deleted.emit());
  }

  get totalTime() {
    return this.recipe.prepTimeMinutes + this.recipe.cookTimeMinutes;
  }
}
