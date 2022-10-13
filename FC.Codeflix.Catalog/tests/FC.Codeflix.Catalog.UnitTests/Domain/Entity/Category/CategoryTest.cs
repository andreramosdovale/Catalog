using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;
using FC.Codeflix.Catalog.Domain.Exceptions;
using System.Xml.Linq;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;
public class CategoryTest
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        var validData = new
        {
            Name = "category name",
            Description = "description name"
        };
        var dateTimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validData.Name, validData.Description);
        var dateTimeAfter = DateTime.Now;

        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(default(Guid), category.Id);
        Assert.NotEqual(default(DateTime), category.CreatedAt);
        Assert.True(category.CreatedAt > dateTimeBefore);
        Assert.True(category.CreatedAt < dateTimeAfter);
        Assert.True(category.IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validData = new
        {
            Name = "category name",
            Description = "description name"
        };
        var dateTimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validData.Name, validData.Description, isActive);
        var dateTimeAfter = DateTime.Now;

        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(default(Guid), category.Id);
        Assert.NotEqual(default(DateTime), category.CreatedAt);
        Assert.True(category.CreatedAt > dateTimeBefore);
        Assert.True(category.CreatedAt < dateTimeAfter);
        Assert.Equal(isActive, category.IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpy))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void InstantiateErrorWhenNameIsEmpy(string? name)
    {
        Action action = () => new DomainEntity.Category(name!, "Category description");

        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Name should not be empty or null", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        Action action = () => new DomainEntity.Category("Category name", null!);

        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Description should not be null", exception.Message);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThan3Charecters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("ab")]
    public void InstantiateErrorWhenNameIsLessThan3Charecters(string name)
    {
        Action action = () => new DomainEntity.Category(name, "Category description");

        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Name should be at leats 3 characters long", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Charecters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThan255Charecters()
    {
        var name = string.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());
        Action action = () => new DomainEntity.Category(name, "Category description");

        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Name should be at less or equal 255 characters long", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Charecters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThan10_000Charecters()
    {
        var description = string.Join(null, Enumerable.Range(0, 10_001).Select(_ => "a").ToArray());
        Action action = () => new DomainEntity.Category("Category name", description);

        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Description should be at less or equal 10.000 characters long", exception.Message);
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        var validData = new
        {
            Name = "category name",
            Description = "description name"
        };

        var category = new DomainEntity.Category(validData.Name, validData.Description, false);
        category.Activate();

        Assert.True(category.IsActive);
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        var validData = new
        {
            Name = "category name",
            Description = "description name"
        };

        var category = new DomainEntity.Category(validData.Name, validData.Description, true);
        category.Deactivate();

        Assert.False(category.IsActive);
    }

    [Theory(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData("new Description")]
    public void Update(string? newDescription)
    {
        var validData = new
        {
            Name = "category name",
            Description = "description name"
        };
        var newValues = new
        {
            Name = "new Name",
            Description = newDescription!
        };

        var category = new DomainEntity.Category(validData.Name, validData.Description);
        category.Update(newValues.Name, newValues.Description);

        Assert.Equal(category.Name, newValues.Name);
        Assert.Equal(category.Description, newValues.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        var validData = new
        {
            Name = "category name",
            Description = "description name"
        };
        var category = new DomainEntity.Category(validData.Name, validData.Description);
        var currentDescription = category.Description;
        var newValues = new
        {
            Name = "new Name"
        };
       
        category.Update(newValues.Name);

        Assert.Equal(category.Name, newValues.Name);
        Assert.Equal(category.Description, currentDescription);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpy))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void UpdateErrorWhenNameIsEmpy(string? name)
    {
        var category = new DomainEntity.Category("Category name", "Category description");

        Action action = () => category.Update(name!);

        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Name should not be empty or null", exception.Message);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLessThan3Charecters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("ab")]
    public void UpdateErrorWhenNameIsLessThan3Charecters(string name)
    {
        var category = new DomainEntity.Category("Category name", "Category description");

        Action action = () => category.Update(name);

        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Name should be at leats 3 characters long", exception.Message);
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThan255Charecters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThan255Charecters()
    {
        var name = string.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());
        var category = new DomainEntity.Category("Category name", "Category description");

        Action action = () => category.Update(name);

        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Name should be at less or equal 255 characters long", exception.Message);
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThan10_000Charecters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThan10_000Charecters()
    {
        var description = string.Join(null, Enumerable.Range(0, 10_001).Select(_ => "a").ToArray());
        var category = new DomainEntity.Category("Category name", "Category description");

        Action action = () => category.Update("Category name", description);

        var exception = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Description should be at less or equal 10.000 characters long", exception.Message);
    }
}
