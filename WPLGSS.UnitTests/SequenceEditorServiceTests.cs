using FakeItEasy;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Models;
using WPLGSS.ViewModels;
using Xunit;

namespace WPLGSS.Services.UnitTests
{
    public class SequenceEditorServiceTests
    {
        [Fact]
        public void OpenNewSequenceInSequenceEditorServiceAddsView()
        {
            var regionManager = A.Fake<IRegionManager>();
            var regionName = "Region";
            var path = "Path";
            var sequence = new SequenceViewModel(new Sequence());
            var region = new Region();
            var regionCollection = A.Fake<IRegionCollection>();

            A.CallTo(() => regionCollection[regionName]).Returns(region);
            A.CallTo(() => regionManager.Regions).Returns(regionCollection);

            var service = new SequenceEditorService(regionManager);

            service.OpenSequenceInRegion(regionName, path, sequence);

            Assert.Single(region.Views);
            Assert.Single(region.ActiveViews);
            Assert.IsType<SequenceFile>(region.ActiveViews.First());
            var view = (SequenceFile)region.ActiveViews.First();
            Assert.Equal(path, view.Path);
            Assert.Equal(sequence, view.Sequence);
        }

        [Fact]
        public void OpenOpenedSequenceActivatesExistingSequence()
        {
            var regionManager = A.Fake<IRegionManager>();
            var regionName = "Region";
            var path = "Path";
            var sequence = new SequenceViewModel(new Sequence());
            var region = new Region();
            region.Add(new SequenceFile(path, sequence), path);
            var regionCollection = A.Fake<IRegionCollection>();

            A.CallTo(() => regionCollection[regionName]).Returns(region);
            A.CallTo(() => regionManager.Regions).Returns(regionCollection);

            var service = new SequenceEditorService(regionManager);


            service.OpenSequenceInRegion(regionName, path, sequence);

            Assert.Single(region.Views);
            Assert.Single(region.ActiveViews);
            Assert.IsType<SequenceFile>(region.ActiveViews.First());
            var view = (SequenceFile)region.ActiveViews.First();
            Assert.Equal(path, view.Path);
            Assert.Equal(sequence, view.Sequence);
        }

        [Fact]
        public void UpdateViewNameRemovesOldSequenceAndAddsNewOne()
        {
            var regionManager = A.Fake<IRegionManager>();
            var regionName = "Region";
            var oldPath = "Path";
            var newPath = "Path2";
            var sequence = new SequenceViewModel(new Sequence());
            var region = new Region();
            var sequenceFile = new SequenceFile(oldPath, sequence);
            region.Add(sequenceFile, oldPath);
            var regionCollection = A.Fake<IRegionCollection>();

            A.CallTo(() => regionCollection[regionName]).Returns(region);
            A.CallTo(() => regionManager.Regions).Returns(regionCollection);

            var service = new SequenceEditorService(regionManager);

            service.UpdateViewNameForSequence(regionName, newPath, sequenceFile);

            Assert.Single(region.Views);
            Assert.Equal(sequence, ((SequenceFile)region.GetView(newPath)).Sequence);
            Assert.Null(region.GetView(oldPath));
        }
    }
}
