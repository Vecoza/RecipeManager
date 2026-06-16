import { RecipeListItem } from './recipe.models';

export interface PantryItem {
  id: string;
  name: string;
  quantity: number;
  unit: string;
}

export interface CreatePantryItemDto {
  name: string;
  quantity: number;
  unit: string;
}

export interface RecipeMatchDto {
  recipe: RecipeListItem;
  matchedCount: number;
  totalCount: number;
  matchPercent: number;
}

export interface PantryMatchResult {
  exactMatches: RecipeMatchDto[];
  partialMatches: RecipeMatchDto[];
}

export interface GeneratedRecipeDto {
  title: string;
  description: string;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  servings: number;
  ingredients: { name: string; quantity: number; unit: string }[];
  steps: { instruction: string }[];
}
